using Compotitor.Enums;
using Compotitor.Model;
using System;
using System.Collections.Generic;

namespace Compotitor.Services
{
    public partial class CompotitorService
    {
        private bool EndGame = false;
        private Dictionary<int, TreeNode> AllLeaves = new Dictionary<int, TreeNode>();
        private List<TreeNode> GstTargets = new List<TreeNode>();
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
        private List<int> RoadMap = new List<int>();
        private int Position;
        private List<int> RottenBoxes = new List<int>();
        private static string PlayerId { get; set; }

        private HashSet<int> OpponentPositions;
        private Dictionary<string, Player> PlayerMap;
        private bool GameStart = false;
        private bool CanBomb = false;

        private HashSet<int> BombPositions = new HashSet<int>();
        private HashSet<int> BombSpots = new HashSet<int>();
        private HashSet<int> BombDangers = new HashSet<int>();
        private List<int> NewOpponentBombs = new List<int>();
        private List<int> OldOpponentBombs = new List<int>();
        private int[] BombMap = new int[3025];
        private bool HaltSignal = false;
        private DateTimeOffset HaltSignalTime;
        private bool CanMove = true;
        private bool InDanger = false;
        private bool WaitForNextStop = false;
        private TreeNode PivotNode;

        public void SetPlayerId(string playerId)
        {
            PlayerId = playerId;
        }
        public static void SetMapWidth(int mapWidth)
        {
            MapWidth = mapWidth;
        }

        private HashSet<MapCell> AllCellTypes = new HashSet<MapCell>()
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

        private static SocketIOClient.SocketIO Socket;
        public void SetSocket(SocketIOClient.SocketIO socket)
        {
            Socket = socket;
        }


        private List<int> ReachableCells =  new List<int>();
        private List<int> EmptyCells = new List<int>();
        private List<int> Eggs = new List<int>();// Change
        private DateTime DestinationStart;

    }

}