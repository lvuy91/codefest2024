using CodeFest24.Enums;
using CodeFest24.Model;
using Microsoft.Graph.Models;
using System.IO;

namespace CodeFest24.Services
{
    public partial class Main
    {

        #region
        public void ParseTicktack(TicktackResponse res)
        {
            var mapInfo = res.MapInfo;
            MapInfo = mapInfo;
            TickId++;
            Map = mapInfo.Map;
            FlatMap = mapInfo.Map.SelectMany(x => x).ToList();

            CheckForSpecialSpot(FlatMap);

            MapWidth = mapInfo.Size.Cols;
            MapHeight = mapInfo.Size.Rows;

            // Tìm người chơi hiện tại
            var currentPlayer = mapInfo.Players.FirstOrDefault(p => PlayerId == p.Id);
            Player = new GamePlayer(currentPlayer);

            if (currentPlayer != null)
            {
                var p = currentPlayer.CurrentPosition;
                Position = To1dPos(p.Col, p.Row);
            }

            // Xử lý vị trí đối thủ
            var opponents = mapInfo.Players.Where(p => !PlayerId.Contains(p.Id));
            OpponentPositions = new HashSet<int>();

            foreach (var opponent in opponents)
            {
                var p = opponent.CurrentPosition;
                OpponentPositions.Add(To1dPos(p.Col, p.Row));
                //GstEggBeingAttacked = opponent.GstEggBeingAttacked;
            }

            // Cập nhật playerMap
            var PlayerMap = mapInfo.Players.ToDictionary(p => p.Id);

            var SpoilDic = new Dictionary<ESpoilType, List<int>>();

            // Xử lý spoils (mystics và eggs)
            var spoilsDic = res.MapInfo.Spoils.GroupBy(x => x.SpoilType)
                .ToDictionary(x => x.Key, x => x.Select(y => To1dPos(y.Col, y.Row)));

            // Reset các collection

            // Xử lý bombs
            var bombs = mapInfo.Bombs.OrderByDescending(b => b.RemainTime);
            foreach (var bomb in bombs)
            {
                var bombPos = To1dPos(bomb.Col, bomb.Row);
                var bombSpots = GetBombSpots(bombPos, bomb.PlayerId);

                BombPositions.Add(bombPos);
                BombSpots.UnionWith(bombSpots);

                if (bomb.RemainTime < DangerBombTime)
                {
                    BombDangers.UnionWith(bombSpots);
                }

                foreach (var spot in bombSpots)
                {
                    BombMap[spot] = bomb.RemainTime;
                }

                if (!PlayerId.Contains(bomb.PlayerId))
                {
                    NewOpponentBombs.Add(bombPos);
                }
            }

            // Kiểm tra bomb mới
            var hasNewBomb = NewOpponentBombs.Except(OldOpponentBombs).Any();
            OldOpponentBombs = NewOpponentBombs;

            if (hasNewBomb && RoadMap.Any(c => BombSpots.Contains(c)))
            {
                HaltSignal = true;
                //Socket.Emit("drive player", new { direction = "x" });
                HaltSignalTime = DateTime.Now;
            }

            // Cập nhật trạng thái game
            if (!GameStart)
            {
                CanMove = true;
                GameLock = false;
            }

            var playerPosition = Player.Position;
            InDanger = BombSpots.Contains(playerPosition);
            ReadMap1();
            // [this.spot1, this.gstEgg1] = this.ReadMap2();
            Run();
        }





        // Phương thức kiểm tra nếu một vị trí tốt hơn một vị trí khác
        private bool CheckForGoodSpot(TreeNode spot, TreeNode goodSpot)
        {
            // Tính toán điểm cho mỗi vị trí
            double points = spot.Boxes * spot.IsolatedBoxes * 0.5;
            double goodSpotPoints = goodSpot.Boxes * goodSpot.IsolatedBoxes * 0.5;

            // Nếu không có sức mạnh đầy đủ, tính toán điểm khác
            if (!IsFullPower())
            {
                points = spot.Boxes * 0.7 + spot.BonusPoints * 0.2 * spot.IsolatedBoxes * 0.5;
                goodSpotPoints = goodSpot.Boxes * 0.7 + goodSpot.BonusPoints * 0.2 * goodSpot.IsolatedBoxes * 0.5;
            }

            // Trả về true nếu điểm của spot lớn hơn điểm của goodSpot
            return goodSpotPoints < points;
        }

        // Method to check for special spots on the map
        private void CheckForSpecialSpot(List<int> map)
        {
            for (int c = 0; c < map.Count; c++)
            {
                var cellType = map[c];
                if (!AllCellTypes.Contains((MapCell)cellType))
                {
                    // Mark the special spot (possibly a specific map object like DragonEggGST or another target)
                    SpecialSpot = c;
                    break;
                }
            }
        }


        // GetPathFromRoot
        public string GetPathFromRoot(TreeNode node)
        {
            if (node.Parent == null)
            {
                return "";
            }

            return GetPathFromRoot(node.Parent) + node.Dir;
        }

        public void Run()
        {
            if (GameLock == false)
            {
                GameLock = true;
                int startPosition = Player.Position;

                if (RoadMap.Count > 0 && RoadMap[0] == Player.Position)
                {
                    RoadMap.RemoveAt(0);
                    IdleStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    if (RoadMap.Count == 0)
                    {
                        RecheckCanBomb();
                    }
                }

                if (RoadMap.Count > 0 && DateTimeOffset.Now.ToUnixTimeMilliseconds() - IdleStart > TimeInOneCell)
                {
                    Console.WriteLine("idling... reset the destination");
                    RoadMap.Clear();
                    RecheckCanBomb();
                }

                if (WaitForNextStop && (DateTimeOffset.Now.ToUnixTimeMilliseconds() -
                    HaltSignalTime.ToUnixTimeMilliseconds() > TimeInOneCell))
                {
                    WaitForNextStop = false;
                    RoadMap.Clear();
                    RecheckCanBomb();
                }
                HandleInDanger();
                GameLock = false;
            }
        }

        private void HandleInDanger()
        {
            if (RoadMap.Count > 0) { return; }
            {
                if (InDanger)
                {
                    GotoSafePlace();
                }
                else if (CountGoExploreSpecialSpot < 2 && SpecialSpot.HasValue && ReachableCells.Contains(SpecialSpot.Value))
                {
                    if (NearTheSpecialSpot())
                    {
                        TryBombSpecialSpot();
                    }
                    else
                    {
                        GoExploreSpecialSpot();
                    }
                }
                else if (CanBomb)
                {
                    GoBomb();
                }
                else
                {
                    GoToGoodSpot();
                }
            }
        }


        public void GotoSafePlace()
        {
            Console.WriteLine("Go to safe place");
            var root = new TreeNode(Player.Position);
            var node = GetAvoidBomb(root, Map, BombSpots);

            if (node != null)
            {
                //console.log('extend path', extendPath);
                string path = GetPathFromRoot(node);
                DrivePlayer(path, node);
                StoreRoadMap(new List<TreeNode> { node });
            }
        }


        public void GoBomb()
        {
            TreeNode standNode = null;
            //bool withMystic = false;
            //bool withTeleport = false;

            //// Kiểm tra các điểm đặc biệt và gán giá trị cho standNode
            //if (Spot1 != null)
            //{
            //    standNode = Spot1;
            //    withMystic = false; // Không có Mystic
            //}
            //else if (Spot2 != null)
            //{
            //    standNode = Spot2;
            //    withMystic = true; // Có Mystic
            //}
            //else if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - IdleStart > TimeInOneCell * 22 && Spot3 != null)
            //{
            //    standNode = Spot3;
            //    withMystic = true; // Có Mystic
            //    withTeleport = true; // Có Teleport
            //}
            //else if (GstEgg1 != null)
            //{
            //    standNode = GstEgg1;
            //    withMystic = false; // Không có Mystic
            //}
            //else if (GstEgg2 != null)
            //{
            //    standNode = GstEgg2;
            //    withMystic = true; // Có Mystic
            //}
            //else if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - IdleStart > TimeInOneCell * 22 && GstEgg3 != null)
            //{
            //    standNode = GstEgg3;
            //    withMystic = true; // Có Mystic
            //    withTeleport = true; // Có Teleport
            //}

            // Nếu standNode đã được xác định
            if (standNode != null)
            {
                // Tạo TreeNode từ standNode
                TreeNode node = new TreeNode(standNode.Val);

                // Tìm kiếm nơi an toàn để đặt bom, tránh các khu vực có bom
                var extendPath = FindSafePlace(standNode.Val, new HashSet<int>(
                    this.GetBombSpots(standNode.Val, PlayerId)
                    .Union(this.BombSpots)
                ), standNode.Distance);

                // Nếu tìm được đường đi an toàn
                if (extendPath != null)
                {
                    // Lấy hướng đi từ gốc đến điểm hiện tại
                    string direction = GetPathFromRoot(standNode);

                    // Lấy đường đi tiếp theo từ extendPath
                    string tailPath = GetPathFromRoot(extendPath);

                    // Điều khiển nhân vật đi theo đường đi đã tìm được
                    DrivePlayer(direction + 'b' + tailPath, standNode);

                    // Lưu lại đường đi vào RoadMap
                    StoreRoadMap(new List<TreeNode> { extendPath, standNode });

                    // Không cho phép đặt bom ngay lập tức (tạm thời vô hiệu hóa đặt bom)
                    // this.CanBomb = false;
                }
            }
        }


        // Phương thức GoEatSomeEggs - di chuyển đến một quả trứng có thể ăn được
        public void GoEatSomeEggs()
        {
            // Tạo một node gốc với vị trí của người chơi
            var root = new TreeNode(Player.Position, null, null);

            // Lấy đường đi đến quả trứng có thể ăn được
            var node = GetEditableEggsPath(root, Map);
            string path = string.Empty;

            if (node != null)
            {
                // Lấy đường đi từ gốc đến node
                path = GetPathFromRoot(node);
                int pathLen = path.Length;

                // Nếu game chưa kết thúc và độ dài đường đi lớn hơn 3, dừng lại
                if (!EndGame && pathLen > 3)
                {
                    return;
                }

                // Di chuyển người chơi theo đường đi đã tính toán
                if (!string.IsNullOrEmpty(path))
                {
                    DrivePlayer(path, node);  // Di chuyển người chơi
                    StoreRoadMap(new List<TreeNode> { node });  // Lưu lại đường đi
                }
            }
        }

        public void GetAllLeaves(TreeNode startNode, List<List<int>> map)
        {
            AllLeaves = new Dictionary<int, TreeNode>();
            GstTargets = new List<TreeNode>();

            ScanMap(startNode, map, (currentNode) =>
            {
                // Nếu ô hiện tại thuộc vị trí bomb, ngừng xử lý tại node này
                if (BombSpots.Contains(currentNode.Val))
                {
                    return new NodeDto[1] { new NodeDto(null, true) };
                }

                // Nếu ô hiện tại có trứng, cộng điểm bonus
                if (Eggs.Contains(currentNode.Val))
                {
                    currentNode.BonusPoints += 1;
                }

                // Kiểm tra số hộp bị ảnh hưởng
                CountBoxHits(currentNode);

                // Nếu node có hộp và không bị đánh dấu tránh, thêm vào AllLeaves
                if (currentNode.Boxes > 0 && !currentNode.AvoidThis)
                {
                    AllLeaves[currentNode.Val] = currentNode;
                }

                // Nếu node này được đánh dấu tấn công, thêm vào danh sách mục tiêu tấn công
                if (currentNode.AttackThis)
                {
                    GstTargets.Add(currentNode);
                }

                // Trả về tiếp tục duyệt map
                return new NodeDto[1] { new NodeDto(null, false) };
            });
        }



        public object[] ScanRawMap(TreeNode startNode, List<List<int>> map, Func<TreeNode, object[]> callback, bool withTeleport = false)
        {
            // Dùng thuật toán tìm kiếm theo chiều rộng (BFS) để quét qua bản đồ
            Queue<TreeNode> queue = new Queue<TreeNode>();
            HashSet<int> visited = new HashSet<int> { startNode.Val };

            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                TreeNode currentNode = queue.Dequeue();

                // Gọi callback để xử lý vị trí hiện tại
                if (callback != null)
                {
                    object[] result = callback(currentNode);
                    bool ignoreThisNode = (bool)result[1];
                    if (ignoreThisNode)
                    {
                        continue;
                    }
                    if (result[0] != null)
                    {
                        return result;
                    }
                }

                // Lấy các ô lân cận của vị trí hiện tại
                List<int> neighbors = GetNeighborNodes(currentNode.Val);

                // Kiểm tra các ô lân cận
                foreach (int neighbor in neighbors)
                {
                    int cellValue = map[neighbor / MapWidth][neighbor % MapWidth];

                    // Nếu ô là đường đi hoặc co the di chuyen duoc (nếu có)
                    if (cellValue == (int)MapCell.EmptyCell || (cellValue == (int)MapCell.Balk))
                    {
                        // Nếu chưa thăm vị trí này, thêm vào hàng đợi
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            string dir = (neighbors.IndexOf(neighbor) + 1).ToString();
                            TreeNode neighborNode = new TreeNode(neighbor, dir, currentNode);
                            currentNode.Children.Add(neighborNode);
                            queue.Enqueue(neighborNode);
                        }
                    }
                }
            }

            return null;
        }



        public object[] ScanMap(TreeNode startNode, List<List<int>> map, Func<TreeNode, object[]> callback)
        {
            // Quét qua bản đồ để tìm các vị trí hợp lệ và gọi callback nếu cần
            return ScanRawMap(startNode, map, (currentNode) =>
            {
                int loc = currentNode.Val;

                // Nếu vị trí này là đối thủ thì bỏ qua
                if (OpponentPositions.Contains(loc))
                {
                    return new object[] { null, true };
                }

                // Nếu là vùng nguy hiểm do bom, bỏ qua
                if (BombDangers.Contains(loc))
                {
                    return new object[] { null, true };
                }

                // Gọi callback để xử lý vị trí
                if (callback != null)
                {
                    return callback(currentNode);
                }

                return new object[] { null, false };
            });
        }

        public TreeNode GetAvoidBomb(TreeNode startNode, List<List<int>> map, HashSet<int> bombSpots)
        {
            HashSet<TreeNode> goodSpots = new HashSet<TreeNode>();
            int limit = 20;

            // Quét qua bản đồ để tìm các vị trí hợp lệ
            ScanRawMap(startNode, map, (currentNode) =>
            {
                int loc = currentNode.Val;

                // Nếu vị trí này là đối thủ thì bỏ qua
                if (OpponentPositions.Contains(loc))
                {
                    return new object[] { null, true };
                }

                // Nếu không phải là vị trí khởi đầu và có bom tại vị trí này, bỏ qua
                if (startNode.Val != loc && BombPositions.Contains(loc))
                {
                    return new object[] { null, true };
                }

                // Nếu không phải vị trí khởi đầu và là vùng nguy hiểm do bom, bỏ qua
                if (startNode.Val != loc && BombDangers.Contains(loc))
                {
                    return new object[] { null, true };
                }

                // Nếu không phải là vị trí bom, hoặc là cổng dịch chuyển và cho phép dịch chuyển
                if (!bombSpots.Contains(loc) || (FlatMap[loc] == (int)MapCell.Balk || FlatMap[loc] == (int)MapCell.EmptyCell))
                {
                    // Nếu có trứng, tăng điểm thưởng
                    if (Eggs.Contains(loc))
                    {
                        currentNode.BonusPoints += 1;
                    }

                    // Đếm số hộp (Box) tại vị trí này
                    CountBoxHits(currentNode);

                    // Kiểm tra nếu là vị trí tốt
                    bool isGoodSpot1 = currentNode.Boxes > 0 && !currentNode.AvoidThis;
                    bool isGoodSpot2 = currentNode.AttackThis;

                    // Nếu chưa có vị trí tốt hoặc thỏa mãn điều kiện, thêm vào danh sách vị trí tốt
                    if (goodSpots.Count == 0 || isGoodSpot1 || isGoodSpot2)
                    {
                        goodSpots.Add(currentNode);
                    }

                    // Nếu đã tìm đủ số lượng vị trí tốt, dừng lại
                    if (--limit <= 0)
                    {
                        return new object[] { true, false };
                    }
                }

                return new object[] { null, false };
            });

            // Giới hạn khoảng cách nếu có pivotNode
            int limitDistance = int.MaxValue;
            if (PivotNode != null)
            {
                limitDistance = PivotNode.Distance + AroundPivotLimit;
            }

            // Tìm vị trí tốt nhất trong các vị trí đã tìm được
            TreeNode goodSpot = null;
            bool foundOpponentEgg = false;

            foreach (TreeNode spot in goodSpots)
            {
                if (!foundOpponentEgg && spot.AttackThis)
                {
                    foundOpponentEgg = true;
                    goodSpot = spot;
                    continue;
                }

                if (goodSpot == null)
                {
                    goodSpot = spot;
                    continue;
                }

                // Nếu khoảng cách của vị trí này vượt quá giới hạn, dừng tìm
                if (spot.Distance > limitDistance)
                {
                    break;
                }

                // Tính toán điểm tổng (hộp + điểm thưởng) để chọn ra vị trí tốt nhất
                int points = spot.Boxes + spot.BonusPoints;
                if ((goodSpot.Boxes + goodSpot.BonusPoints) < points)
                {
                    goodSpot = spot;
                }
            }

            return goodSpot;
        }

        // Phương thức kiểm tra nếu vị trí hiện tại gần vị trí đặc biệt
        private bool NearTheSpecialSpot()
        {
            // Kiểm tra xem có vị trí đặc biệt không
            if (SpecialSpot.HasValue)
            {
                // Lấy các ô lân cận của vị trí đặc biệt
                var neighbors = GetNeighborNodes(SpecialSpot.Value);

                // Kiểm tra xem vị trí người chơi có nằm trong các ô lân cận không
                return neighbors.Contains(Player.Position);
            }
            return false;
        }

        // Phương thức thử đặt bom ở vị trí đặc biệt
        private void TryBombSpecialSpot()
        {
            // Console chỉ cần sử dụng để gỡ lỗi
            // Console.WriteLine("try bomb special spot");

            // Gửi lệnh để di chuyển người chơi và đặt bom
            // (Giả định đây là một hành động trong game với "b" là đặt bom)
            Socket.EmitAsync("drive player", new { direction = "b" }).Wait();

            // Tăng số lần khám phá vị trí đặc biệt
            CountGoExploreSpecialSpot += 1;
        }

        // Phương thức CountBoxHits - đếm số hộp và hộp bị cô lập trong khu vực xung quanh node
        public void CountBoxHits(TreeNode node)
        {
            int loc = node.Val;
            int playerPower = PlayerMap.ContainsKey(PlayerId) ? PlayerMap[PlayerId].Power : 1;
            int boxes = 0;
            int isolatedBoxes = 0;

            var allDirections = new List<int> { -1, 1, -MapWidth, MapWidth };

            foreach (var direction in allDirections)
            {
                for (int i = 1; i <= playerPower; i++)
                {
                    int p = loc + direction * i;  // Tính toán vị trí mới sau khi di chuyển
                    MapCell cellType = (MapCell)FlatMap[p];  // Lấy loại ô tại vị trí mới

                    if (cellType == MapCell.Wall)
                    {
                        break;  // Dừng lại nếu gặp phải bức tường
                    }

                    if (cellType == MapCell.BrickWall)
                    {
                        node.AttackThis = true;  // Nếu là quả trứng của mục tiêu, tấn công nó
                        break;
                    }

                    if (cellType == MapCell.Balk && !RottenBoxes.Contains(p))
                    {
                        if (IsIsolatedBalk(p))
                        {
                            isolatedBoxes += 1;  // Nếu Balk bị cô lập, đếm nó
                        }
                        else
                        {
                            boxes += 1;  // Nếu Balk không bị cô lập, đếm nó
                        }
                        break;
                    }

                    if (OpponentPositions.Contains(p))
                    {
                        node.PlayerFootprint = true;  // Nếu đối thủ đi qua ô này, đánh dấu là dấu vết của đối thủ
                    }
                }
            }

            node.Boxes = boxes;  // Lưu số lượng hộp
            node.IsolatedBoxes = isolatedBoxes;  // Lưu số lượng hộp bị cô lập
        }

        // Phương thức GetBombSpots - tìm các vị trí có thể đặt bom xung quanh một điểm
        public HashSet<int> GetBombSpots(int pos, string playerId)
        {
            int playerPower = 3;

            // Tìm thông tin người chơi từ playerId
            var player = PlayerMap.ContainsKey(playerId) ? PlayerMap[playerId] : null;
            if (player != null)
            {
                playerPower = player.Power;  // Sử dụng sức mạnh của người chơi nếu có
            }

            // Các ô có thể đi qua
            var passThroughCells = new HashSet<MapCell> {
                MapCell.EmptyCell, MapCell.CellsDestroyed, MapCell.Balk, MapCell.CellsDestroyed, MapCell.GodBadge };
            var bombSpots = new HashSet<int> { pos };
            var allDirections = new List<int> { -1, 1, -MapWidth, MapWidth };  // Các hướng di chuyển (trái, phải, lên, xuống)

            foreach (var direction in allDirections)
            {
                for (int i = 1; i <= playerPower; i++)
                {
                    int p = pos + direction * i;  // Tính toán vị trí mới sau khi di chuyển
                    MapCell cellType = (MapCell)FlatMap[p];  // Lấy loại ô tại vị trí mới

                    if (!passThroughCells.Contains(cellType))
                    {
                        if (cellType == MapCell.Balk)
                        {
                            RottenBoxes.Add(p);  // Nếu là Balk (hộp mục), đánh dấu là mục nát
                        }
                        break;  // Dừng lại nếu gặp phải ô không thể đi qua
                    }
                    bombSpots.Add(p);  // Thêm vị trí vào danh sách các ô có thể đặt bom
                }
            }

            return bombSpots;
        }

        #endregion
        #region
        private bool IsFullPower()
        {
            return Player.PlayerInfo.HasTransform;
        }


        private void RecheckCanBomb()
        {
            bool canBomb = MapInfo.Bombs.Count(b => PlayerId.Contains(b.PlayerId)) == 0;
            if (canBomb)
            {
                CanBomb = canBomb;
            }
        }

        private int To2dPos(int pos)
        {
            int y = pos / MapWidth;
            int x = pos % MapWidth;
            return x + y * MapWidth;
        }

        public int To1dPos(int x, int y)
        {
            return y * MapWidth + x;
        }

        // Phương thức GetNeighborNodes - tìm các ô láng giềng của một node
        public List<int> GetNeighborNodes(int val)
        {
            int cols = MapWidth;
            return new List<int>
           {
               val - 1,  // Ô bên trái
               val + 1,  // Ô bên phải
               val - cols,  // Ô phía trên
               val + cols,  // Ô phía dưới
           };
        }

        // Phương thức kiểm tra nếu một ô là Balk bị cô lập
        private bool IsIsolatedBalk(int pos)
        {
            int cols = MapWidth;

            // Lấy các vị trí lân cận của ô hiện tại
            var surroundSpots = new List<int>
           {
               pos - 1,
               pos + 1,
               pos - cols,
               pos - cols - 1,
               pos - cols + 1,
               pos + cols,
               pos + cols - 1,
               pos + cols + 1
            };

            // Kiểm tra nếu các ô lân cận là Balk, Balk hoặc là vị trí mục tiêu
            foreach (var spot in surroundSpots)
            {
                if (FlatMap[spot] == (int)MapCell.Balk || FlatMap[spot] == (int)MapCell.CellsDestroyed
                    //|| spot == TargetGst
                    )
                {
                    return false;
                }
            }
            return true;
        }


        public void ReadMap1()
        {
            PivotNode = null;
            var attackThisNodes = new List<TreeNode>(); // List để lưu các nút cần tấn công
            var map = this.FlatMap;
            var position = Player.Position;
            var startNode = new TreeNode(position); // Tạo nút bắt đầu từ vị trí của người chơi
            var queue = new Queue<TreeNode>();
            queue.Enqueue(startNode);
            var visited = new HashSet<int> { position }; // Lưu các vị trí đã thăm để tránh lặp lại

            // Duyệt qua các nút trong queue để khám phá bản đồ
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue(); // Lấy nút hiện tại ra khỏi hàng đợi
                this.ReachableCells.Add(currentNode.Val); // Thêm vào các ô có thể đến
                this.CountBoxHits(currentNode); // Tính toán số lượng hộp

                // Nếu không có hộp và không cần tấn công, đánh dấu là ô trống
                if (currentNode.Boxes == 0 && !currentNode.AttackThis)
                {
                    this.EmptyCells.Add(currentNode.Val);
                }

                // Nếu tìm thấy nút có hộp và chưa tìm thấy pivot, gán nó làm pivot
                if (currentNode.Boxes > 0 && PivotNode == null)
                {
                    PivotNode = currentNode;
                }

                // Nếu nút này là nơi cần tấn công, thêm vào danh sách các nút tấn công
                if (currentNode.AttackThis)
                {
                    attackThisNodes.Add(currentNode);
                }

                // Lấy các nút lân cận của nút hiện tại
                var neighbors = this.GetNeighborNodes(currentNode.Val);
                foreach (var idx in neighbors)
                {
                    var neighbor = neighbors[idx];
                    var cellValue = map[neighbor]; // Giá trị của ô lân cận
                                                   // Nếu ô là đường đi hoặc là hộp đã hỏng, tiếp tục kiểm tra
                    if (cellValue == (int)MapCell.EmptyCell || this.RottenBoxes.Contains(neighbor))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            var dir = (idx + 1).ToString(); // Chuyển chỉ số của hướng
                            var neighborNode = new TreeNode(neighbor, dir, currentNode);
                            currentNode.Children.Add(neighborNode); // Thêm nút con vào nút hiện tại
                            queue.Enqueue(neighborNode); // Đưa nút con vào hàng đợi để tiếp tục duyệt
                        }
                    }
                }
            }

            // Nếu không tìm thấy pivotNode, chọn nút cần tấn công đầu tiên làm pivot
            if (PivotNode == null && attackThisNodes.Count > 0)
            {
                PivotNode = attackThisNodes[0];
            }
        }

        public Tuple<TreeNode, TreeNode> ReadMap2()
        {
            var attackSpots = new List<TreeNode>(); // Danh sách các vị trí cần tấn công
            var gstTargets = new List<TreeNode>(); // Danh sách các mục tiêu GST

            var map = FlatMap;
            var position = Player.Position;
            var startNode = new TreeNode(position);
            var queue = new Queue<TreeNode>();
            queue.Enqueue(startNode);
            var visited = new HashSet<int> { position }; // Lưu các vị trí đã thăm

            // Duyệt qua các nút trong queue để khám phá bản đồ
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                var p = currentNode.Val;

                // Bỏ qua các đối thủ và các vị trí có bom
                if (this.OpponentPositions.Contains(p) || this.BombPositions.Contains(p))
                {
                    continue;
                }


                // Kiểm tra xem có thể đi qua các vị trí có bom không
                if (this.BombMap.Contains(p))
                {
                    var bombTime = this.BombMap[p];
                    if (!CanWalkThrough(bombTime, currentNode.Distance))
                    {
                        continue;
                    }
                }

                this.CountBoxHits(currentNode); // Tính toán số lượng hộp
                                                // Nếu có trứng, cộng điểm thưởng
                if (this.Eggs.Contains(currentNode.Val))
                {
                    currentNode.BonusPoints += 1;
                }

                // Nếu có hộp hoặc hộp bị cô lập, và không cần tránh, thêm vào danh sách các điểm tấn công
                if ((currentNode.Boxes > 0 || currentNode.IsolatedBoxes > 1) && !currentNode.AvoidThis)
                {
                    attackSpots.Add(currentNode);
                }

                // Nếu đây là mục tiêu GST, thêm vào danh sách các mục tiêu GST
                if (currentNode.AttackThis)
                {
                    gstTargets.Add(currentNode);
                }

                // Duyệt qua các ô lân cận
                var neighbors = this.GetNeighborNodes(currentNode.Val);
                foreach (var idx in neighbors)
                {
                    var neighbor = neighbors[idx];
                    var cellValue = map[neighbor];
                    // Nếu là đường đi hoặc cổng teleport, kiểm tra tiếp
                    if (cellValue == (int)MapCell.EmptyCell || cellValue == (int)MapCell.Balk)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            var dir = (idx + 1).ToString();
                            var neighborNode = new TreeNode(neighbor, dir, currentNode);
                            currentNode.Children.Add(neighborNode);
                            queue.Enqueue(neighborNode);
                        }
                    }
                }
            }

            // Xác định điểm tấn công tốt nhất
            TreeNode goodSpot = null;
            int limitDistance = int.MaxValue;
            if (PivotNode != null)
            {
                limitDistance = PivotNode.Distance + AroundPivotLimit; // Giới hạn khoảng cách xung quanh Pivot
            }

            foreach (var spot in attackSpots)
            {
                if (spot.Distance > limitDistance)
                {
                    break;
                }

                // Nếu chưa có điểm tốt, chọn điểm này
                if (goodSpot == null)
                {
                    goodSpot = spot;
                    continue;
                }

                // Nếu điểm có khoảng cách nhỏ hơn 6 và có dấu vết của đối thủ, chọn điểm này
                if (spot.Distance < 6 && spot.PlayerFootprint)
                {
                    goodSpot = spot;
                    Console.WriteLine($"Found opponent at {this.To2dPos(goodSpot.Val)}");
                    break;
                }

                // Kiểm tra điểm này so với điểm tốt hiện tại
                if (this.CheckForGoodSpot(spot, goodSpot))
                {
                    goodSpot = spot;
                }
            }

            // Xác định mục tiêu GST cần tấn công
            TreeNode attackGSTEggSpot = null;
            foreach (var node in gstTargets)
            {
                if (node.Distance > limitDistance)
                {
                    break;
                }
                attackGSTEggSpot = node;
                break;
            }

            return new Tuple<TreeNode, TreeNode>(goodSpot, attackGSTEggSpot);
        }

        private bool CanWalkThrough(int bombTime, int distance)
        {
            // Kiểm tra xem khoảng cách có thể đi qua hay không dựa trên thời gian bom và khoảng cách
            // So với Node.js, ta thay đổi hệ số tính toán cho phù hợp với yêu cầu mới.
            // return distance * 330 + 650 < bombTime;
            return distance * 430 + 450 < bombTime;
        }

        private void DrivePlayer(string path, TreeNode node)
        {
            // Tính chiều dài của đường đi sau khi loại bỏ ký tự 'b'
            int pathLen = path.Count(c => c != 'b');

            // Nếu có đường đi, gửi lệnh di chuyển
            if (!string.IsNullOrEmpty(path))
            {
                // Giả sử bạn đang sử dụng WebSocket hoặc cơ chế tương tự để gửi dữ liệu di chuyển
                // this.socket.Emit("drive player", new { direction = path });
                Console.WriteLine($"Driving player with direction: {path}");
            }

            // Có thể thêm logic cho việc tạm thời dừng di chuyển nếu cần
            // this.canMove = false;
            // this.canMoveHandler = setTimeout(() => this.canMove = true, 250 * pathLen);
        }

        private void StoreRoadMap(List<TreeNode> nodes)
        {
            // Lưu lại bản đồ đường đi, sẽ lưu theo thứ tự từ đích về điểm xuất phát
            this.RoadMap = new List<int>();
            this.DestinationStart = DateTime.Now;

            // Lặp qua các node và thêm vào RoadMap
            foreach (var node in nodes)
            {
                var n = node;
                while (n != null)
                {
                    this.RoadMap.Insert(0, n.Val); // Thêm node vào đầu danh sách
                    n = n.Parent; // Di chuyển lên cha
                }
            }
        }

        private void GoToGoodSpot()
        {
            // Tìm vị trí tốt gần người chơi
            var goodSpot = FindGoodSpot(Player.Position); // Chọn vị trí tốt nhất

            // Nếu tìm thấy vị trí tốt, điều khiển di chuyển
            if (goodSpot != null)
            {
                string path = GetPathFromRoot(goodSpot);
                if (!string.IsNullOrEmpty(path))
                {
                    DrivePlayer(path, goodSpot); // Di chuyển tới vị trí tốt
                    StoreRoadMap(new List<TreeNode> { goodSpot }); // Lưu lại bản đồ đường đi
                }
            }
        }

        public TreeNode FindGoodSpot(int position)
        {
            var goodSpots = new List<TreeNode>();
            var badSpots = new List<TreeNode>();

            int limitDistance = int.MaxValue;
            if (PivotNode != null)
            {
                limitDistance = PivotNode.Distance + AroundPivotLimit;
            }

            var map = FlatMap;
            var startNode = new TreeNode(position);
            var queue = new Queue<TreeNode>();
            queue.Enqueue(startNode);

            var visited = new HashSet<int> { position };
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                int p = currentNode.Val;

                if (currentNode.Distance > limitDistance)
                {
                    break;
                }

                // Skip if opponent is at this position
                if (OpponentPositions.Contains(p))
                {
                    continue;
                }

                // Skip if it's the player's current position and there is a bomb
                if (p != position && BombPositions.Contains(p))
                {
                    continue;
                }
                // Handle bombs at this position
                if (BombMap.Contains(p))
                {
                    int bombTime = BombMap[p];
                    if (!CanWalkThrough(bombTime, currentNode.Distance))
                    {
                        continue;
                    }
                }

                CountBoxHits(currentNode);

                // If this position contains an egg, increase bonus points
                if (Eggs.Contains(currentNode.Val))
                {
                    currentNode.BonusPoints += 1;
                }

                // If the node has certain conditions, add it to the good or bad spots list
                if (goodSpots.Count == 0 || currentNode.BonusPoints > 0 || currentNode.Boxes > 0 || currentNode.IsolatedBoxes > 1 || currentNode.AttackThis || currentNode.PlayerFootprint)
                {
                    if (!BombSpots.Contains(p))
                    {
                        goodSpots.Add(currentNode);
                    }
                    else
                    {
                        badSpots.Add(currentNode);
                    }
                }

                // Explore neighbors
                var neighbors = GetNeighborNodes(currentNode.Val);
                foreach (var neighbor in neighbors)
                {
                    int cellValue = map[neighbor];
                    if (cellValue == (int)MapCell.EmptyCell || cellValue == (int)MapCell.Balk)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            int dir = int.Parse(neighbor.ToString()) + 1;  // Direction logic
                            var neighborNode = new TreeNode(neighbor, dir.ToString(), currentNode);
                            currentNode.Children.Add(neighborNode);
                            queue.Enqueue(neighborNode);
                        }
                    }
                }
            }

            // Find the best good spot from the list
            TreeNode bestSpot = null;
            foreach (var spot in goodSpots)
            {
                if (bestSpot == null || CheckForGoodSpot(spot, bestSpot))
                {
                    bestSpot = spot;
                }
            }

            // If no good spots, pick a bad spot
            if (bestSpot == null)
            {
                foreach (var spot in badSpots)
                {
                    Console.WriteLine("Bad spot", spot);
                    bestSpot = spot;
                }
            }

            return bestSpot;
        }



        public TreeNode GetEditableEggsPath(TreeNode startNode, List<List<int>> map)
        {
            // Quét bản đồ và trả về vị trí có thể chỉnh sửa trứng
            var node = (TreeNode[])ScanMap(startNode, map, (currentNode) =>
            {
                if (BombSpots.Contains(currentNode.Val))
                {
                    return null;
                }

                if (Eggs.Contains(currentNode.Val))
                {
                    return new TreeNode[1] { currentNode };
                }

                return null;
            });

            return node?[0];
        }



        // Method to find a safe place on the map
        private TreeNode FindSafePlace(int position, HashSet<int> dangerSpots, int initDistance = 0)
        {
            var goodSpots = new List<TreeNode>();

            var map = FlatMap;
            var startNode = new TreeNode(position);
            startNode.Distance = initDistance;
            var queue = new Queue<TreeNode>();
            queue.Enqueue(startNode);
            var visited = new HashSet<int> { position };

            // BFS to find safe spots
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                var p = currentNode.Val;

                // Skip if the spot is occupied by an opponent
                if (OpponentPositions.Contains(p))
                {
                    continue;
                }

                // Skip if the spot is a bomb spot
                if (p != position && BombPositions.Contains(p))
                {
                    continue;
                }

                // Skip if there's a bomb and the player can't walk through it
                if (BombMap.Contains(p))
                {
                    var bombTime = BombMap[p];
                    if (!CanWalkThrough(bombTime, currentNode.Distance))
                    {
                        continue;
                    }
                }

                // Count box hits and add bonus points if the spot is an egg
                CountBoxHits(currentNode);
                if (Eggs.Contains(currentNode.Val))
                {
                    currentNode.BonusPoints += 1;
                }

                // If it's a valid good spot, add it to the list
                if (goodSpots.Count == 0 || currentNode.BonusPoints > 0 || currentNode.Boxes > 0 || currentNode.IsolatedBoxes > 1 || currentNode.AttackThis)
                {
                    if (!dangerSpots.Contains(p))
                    {
                        goodSpots.Add(currentNode);
                    }
                }

                // Explore neighboring cells
                var neighbors = GetNeighborNodes(currentNode.Val);
                foreach (var neighbor in neighbors)
                {
                    var cellValue = map[neighbor];
                    if (cellValue == (int)MapCell.EmptyCell || cellValue == (int)MapCell.Balk)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            var dir = int.Parse(neighbor.ToString()) + 1;
                            var neighborNode = new TreeNode(neighbor, dir.ToString(), currentNode);
                            currentNode.Children.Add(neighborNode);
                            queue.Enqueue(neighborNode);
                        }
                    }
                }
            }

            // Select the best good spot based on the conditions
            TreeNode goodSpot = null;
            int firstDistance = int.MaxValue;
            bool foundOpponentEgg = false;

            foreach (var spot in goodSpots)
            {
                if (goodSpot == null)
                {
                    goodSpot = spot;
                    firstDistance = spot.Distance;
                    if (goodSpot.AttackThis)
                    {
                        foundOpponentEgg = true;
                    }
                    continue;
                }

                if (!foundOpponentEgg && spot.AttackThis && spot.Distance < firstDistance + MinimumDistance)
                {
                    foundOpponentEgg = true;
                    goodSpot = spot;
                    continue;
                }

                var points = spot.BonusPoints;
                var goodSpotPoints = goodSpot.BonusPoints;
                if ((goodSpot.Boxes < spot.Boxes || (goodSpotPoints < 1 && goodSpotPoints < points)) && spot.Distance < firstDistance + MinimumDistance)
                {
                    goodSpot = spot;
                }
            }

            return goodSpot;
        }


        // Phương thức để người chơi khám phá vị trí đặc biệt
        private void GoExploreSpecialSpot()
        {
            // Console chỉ cần sử dụng để gỡ lỗi
            // Console.WriteLine("go explore special spot");

            // Tăng số lần khám phá vị trí đặc biệt
            CountGoExploreSpecialSpot += 1;

            // Lấy vị trí đặc biệt làm mục tiêu
            var target = SpecialSpot;

            // Tạo nút khởi điểm từ vị trí mục tiêu
            var startNode = new TreeNode(target.Value);

            // Quét bản đồ (flatMap) để tìm đường đi
            var map = Map;
            var node = (NodeDto[])ScanRawMap(startNode, map, (currentNode) =>
            {
                var loc = currentNode.Val;

                // Kiểm tra nếu đối thủ hoặc bom đang ở gần
                if (OpponentPositions.Contains(loc) ||
                BombDangers.Contains(loc))
                {
                    return new NodeDto[1] { new NodeDto { Node = null, IsOk = true } };
                }

                // Nếu vị trí hiện tại là mục tiêu, trả về nút hiện tại
                if (currentNode.Val == target)
                {
                    return new NodeDto[1] { new NodeDto { Node = currentNode, IsOk = true } };
                }

            return new NodeDto[1] { new NodeDto { Node = null, IsOk = false } };
            });

            // Nếu tìm được đường đi, di chuyển đến đó
            if (node[0].Node != null)
            {
                // Lấy đường đi từ gốc đến nút
                var path = GetPathFromRoot(node[0].Node);

                // Di chuyển người chơi theo đường đi
                DrivePlayer(path, node[0].Node);

                // Lưu lại lộ trình
                StoreRoadMap(new List<TreeNode> { node[0].Node });
            }
        }
        #endregion
    }
}
