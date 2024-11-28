namespace CodeFest24;

internal class main
{
    private static readonly int TimeInOneCell = 500;
    private static readonly int AroundPivotLimit = 5;
    private static readonly int MinimumDistance = 5;
    private static readonly int MinimumEggAttackTime = 15;
    private static readonly int DangerBombTime = 530;
    private static readonly bool CheckFullPower = false;
    private static readonly bool AvoidMystic = true;
    private static readonly bool EnableEndGame = false;

    private enum MapCell
    {
        Road = 0,
        Wall = 1,
        Balk = 2,
        TeleportGate = 3,
        QuarantinePlace = 4,
        DragonEgg = 5
    }

    private enum SpoilType
    {
        Mystic = 6
    }

    private HashSet<MapCell> AllCellTypes = new HashSet<MapCell>
{
    MapCell.Road,
    MapCell.Wall,
    MapCell.Balk,
    MapCell.TeleportGate,
    MapCell.QuarantinePlace,
    MapCell.DragonEgg
};

    private string GetPathFromRoot(TreeNode node)
    {
        if (node.Parent == null)
        {
            return "";
        }

        return GetPathFromRoot(node.Parent) + node.Dir;
    }

    private class TreeNode
    {
        public int Val { get; set; }
        public string Dir { get; set; }
        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; set; }
        public int Boxes { get; set; }
        public int IsolatedBoxes { get; set; }
        public bool AvoidThis { get; set; }
        public bool AttackThis { get; set; }
        public bool PlayerFootprint { get; set; }
        public int Distance { get; set; }
        public int BonusPoints { get; set; }

        public TreeNode(int val, string dir = null, TreeNode parent = null)
        {
            Val = val;
            Dir = dir;
            Parent = parent;
            Children = new List<TreeNode>();
            Boxes = 0;
            IsolatedBoxes = 0;
            AvoidThis = false;
            AttackThis = false;
            PlayerFootprint = false;

            if (parent != null)
            {
                Distance = parent.Distance + 1;
                BonusPoints = parent.BonusPoints;
            }
            else
            {
                Distance = 0;
                BonusPoints = 0;
            }
        }
    }

    private class GamePlayer
    {
        public int Position { get; set; }
        public dynamic PlayerInfo { get; set; }

        public GamePlayer(GameMap gameMap, dynamic playerInfo)
        {
            Position = 0;
            PlayerInfo = playerInfo;
            if (playerInfo != null)
            {
                var p = playerInfo.currentPosition;
                Position = gameMap.To1dPos(p.col, p.row);
            }
        }
    }

    public class GameMap
    {
        private GamePlayer Player;
        // Game state fields
        private string PlayerId;
        private bool GameStart;
        private int TickId;
        private dynamic MapInfo;
        private List<List<int>> Map;
        private List<int> FlatMap;
        private int MapWidth;
        private int MapHeight;
        private Dictionary<string, GamePlayer> PlayerMap;
        private HashSet<int> OpponentPositions;
        private HashSet<int> Mystics;
        private HashSet<int> Eggs;
        private List<int> OldOpponentBombs;
        private List<int> NewOpponentBombs;
        private HashSet<int> BombSpots;
        private HashSet<int> BombDangers;
        private HashSet<int> BombPositions;
        private Dictionary<int, int> BombMap;
        private HashSet<int> RottenBoxes;
        private bool GameLock;
        private bool EndGame;
        private List<int> RoadMap;
        private DateTime DestinationStart;
        private DateTime DestinationEnd;
        private bool HaltSignal;
        private int HaltSignalTime;

        private HashSet<int> ReachableCells;
        private HashSet<int> EmptyCells;
        private TreeNode PivotNode;
        private HashSet<int> EditableEggs;

        private Dictionary<int, TreeNode> AllLeaves;
        private List<TreeNode> GstTargets;
        private TreeNode Spot1;
        private TreeNode Spot2;
        private TreeNode Spot3;
        private TreeNode GstEgg1;
        private TreeNode GstEgg2;
        private TreeNode GstEgg3;
        private bool IgnoreMystic;

        private bool AvoidMystic;
        private bool CanMove;
        private bool InDanger;
        private bool CanBomb;
        private bool HaveEditableEggs;

        private int GstEggBeingAttacked;

        private int CountGoExploreSpecialSpot;


        public GameMap(
            string playerId
            )
        {
            Player = null;
            PlayerId = playerId;
            GameStart = false;
            TickId = 0;
            MapInfo = null;
            Map = new List<List<int>>();
            FlatMap = new List<int>();
            MapWidth = 0;
            MapHeight = 0;
            PlayerMap = new Dictionary<string, GamePlayer>();
            OpponentPositions = new HashSet<int>();
            Mystics = new HashSet<int>();
            Eggs = new HashSet<int>();
            OldOpponentBombs = new List<int>();
            NewOpponentBombs = new List<int>();
            BombSpots = new HashSet<int>();
            BombDangers = new HashSet<int>();
            BombPositions = new HashSet<int>();
            BombMap = new Dictionary<int, int>();
            RottenBoxes = new HashSet<int>();
            GameLock = true;
            EndGame = false;
            RoadMap = new List<int>();
            DestinationStart = DateTime.Now;
            DestinationEnd = DateTime.Now;
            HaltSignal = false;
            HaltSignalTime = -1;

            ReachableCells = new HashSet<int>();
            EmptyCells = new HashSet<int>();
            PivotNode = null;
            EditableEggs = new HashSet<int>();

            AllLeaves = new Dictionary<int, TreeNode>();
            GstTargets = new List<TreeNode>();
            Spot1 = null;
            Spot2 = null;
            Spot3 = null;
            GstEgg1 = null;
            GstEgg2 = null;
            GstEgg3 = null;
            IgnoreMystic = true;

            AvoidMystic = main.AvoidMystic;
            CanMove = false;
            InDanger = false;
            CanBomb = true;
            HaveEditableEggs = false;

            GstEggBeingAttacked = 0;

            CountGoExploreSpecialSpot = 0;
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

                if (WaitForNextStop && DateTimeOffset.Now.ToUnixTimeMilliseconds() - HaltSignalTime > TimeInOneCell)
                {
                    WaitForNextStop = false;
                    RoadMap.Clear();
                    RecheckCanBomb();
                }

                if (RoadMap.Count == 0)
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
                GameLock = false;
            }
        }

        public void ParseTicktack(string id, TicktackResponse res)
        {
            var mapInfo = res.map_info;
            MapInfo = mapInfo;
            TickId++;
            Map = mapInfo.Map;
            FlatMap = Map.SelectMany(x => x).ToList();

            CheckForSpecialSpot(FlatMap);

            MapWidth = mapInfo.Size.Cols;
            MapHeight = mapInfo.Size.Rows;

            // Tìm người chơi hiện tại
            var currentPlayer = mapInfo.Players.FirstOrDefault(p => PlayerId.Contains(p.Id));
            Player = new GamePlayer(this, currentPlayer);

            // Xử lý vị trí đối thủ
            var opponents = mapInfo.Players.Where(p => !PlayerId.Contains(p.Id));
            OpponentPositions = new HashSet<int>();
            GstEggBeingAttacked = 0;

            foreach (var opponent in opponents)
            {
                var p = opponent.CurrentPosition;
                OpponentPositions.Add(To1dPos(p.Col, p.Row));
                GstEggBeingAttacked = opponent.GstEggBeingAttacked;
            }

            // Cập nhật playerMap
            PlayerMap = mapInfo.Players.ToDictionary(p => p.Id);

            // Xử lý DragonEggGST
            PlayerGst = null;
            TargetGst = null;
            foreach (var gstEgg in mapInfo.DragonEggGSTArray)
            {
                if (!PlayerId.Contains(gstEgg.Id))
                {
                    TargetGst = To1dPos(gstEgg.Col, gstEgg.Row);
                }
                if (gstEgg.Id == PlayerId)
                {
                    PlayerGst = To1dPos(gstEgg.Col, gstEgg.Row);
                }
            }

            // Xử lý spoils (mystics và eggs)
            Mystics = new HashSet<int>();
            Eggs = new HashSet<int>();
            foreach (var spoil in mapInfo.Spoils)
            {
                if (AvoidMystic && spoil.SpoilType == SpoilType.Mystic)
                {
                    Mystics.Add(To1dPos(spoil.Col, spoil.Row));
                }
                else
                {
                    Eggs.Add(To1dPos(spoil.Col, spoil.Row));
                }
            }

            // Reset các collection
            RottenBoxes = new HashSet<int>();
            BombSpots = new HashSet<int>();
            BombDangers = new HashSet<int>();
            BombPositions = new HashSet<int>();
            NewOpponentBombs = new List<int>();
            BombMap = new Dictionary<int, int>();

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
            Indanger = BombSpots.Contains(playerPosition);
            EndGame = EnableEndGame && FlatMap.Count(c => c == MapCell.Balk) <= 5;

            UpdateMap(res.Tag, res.PlayerId);

            GameStart = true;
            IgnoreMystic = GstEggBeingAttacked < MinimumEggAttackTime;

            ReadMap1();
            (Spot1, GstEgg1) = ReadMap2();
            (Spot2, GstEgg2) = ReadMap2(true);
            (Spot3, GstEgg3) = ReadMap2(true, true);

            Run();
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
    }
}
