using CodeFest24.Enums;
using CodeFest24.Model;
using SocketIOSharp.Client;

namespace CodeFest24.Services
{
    public partial class Main
    {
        private bool EndGame = false;
        private Dictionary<int, TreeNode> AllLeaves = new Dictionary<int, TreeNode>();
        private List<TreeNode>GstTargets = new List<TreeNode>();
        private const int TimeInOneCell = 500;
        private const int AroundPivotLimit = 5;
        private const int MinimumDistance = 5;
        private const int MinimumEggAttackTime = 15;
        private const int DangerBombTime = 530;
        private const bool CheckFullPower = false;
        private const bool AvoidMystic = true;
        private const bool EnableEndGame = false;
        private static bool GameLock = false;
        private static GamePlayer Player;
        private static long IdleStart = 0;
        private static int MapWidth = 0;
        private MapInfoDto MapInfo;
        private List<List<int>> Map;
        private int TickId = 0;
        private List<int> FlatMap;
        private int MapHeight = 0;
        private List<int> RoadMap = new();
        private int Position;
        private List<int> RottenBoxes = new();
        private static string PlayerId { get; set; }

        private HashSet<int> OpponentPositions;
        private Dictionary<string, Player> PlayerMap;
        private bool GameStart = false;
        private bool CanBomb = false;

        private HashSet<int> BombPositions = new();
        private HashSet<int> BombSpots = new();
        private HashSet<int> BombDangers = new();
        private List<int> NewOpponentBombs = new();
        private List<int> OldOpponentBombs = new();
        private int[] BombMap = new int[3025];
        private bool HaltSignal = false;
        private DateTimeOffset HaltSignalTime;
        private bool CanMove = true;
        private bool InDanger = false;
        private bool WaitForNextStop = false;
        private TreeNode PivotNode;

        public static void SetPlayerId(string playerId)
        {
            PlayerId = playerId;
        }
        public static void SetMapWidth(int mapWidth)
        {
            MapWidth = mapWidth;
        }

        private HashSet<MapCell> AllCellTypes = new()
        {
            MapCell.EmptyCell,
            MapCell.Wall,
            MapCell.Balk,
            MapCell.BrickWall,
            MapCell.PrisonPlace,
            MapCell.GodBadge,
            MapCell.CellsDestroyed
        };


        private int? SpecialSpot;
        private int CountGoExploreSpecialSpot = 0;

        private static SocketIOClient Socket;
        public static void SetSocket(SocketIOClient socket)
        {
            Socket = socket;
        }


        private List<int> ReachableCells = new();
        private List<int> EmptyCells = new();
        private List<int> Eggs = new();// Change
        private DateTime DestinationStart;

    }

}