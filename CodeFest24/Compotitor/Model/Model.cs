using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Compotitor.Model
{
    public class NodeDto
    {
        public NodeDto() { }
        public NodeDto(TreeNode node, bool isOk)
        {
            Node = node;
            IsOk = isOk;
        }
        public TreeNode Node { get; set; }
        public bool IsOk { get; set; }
    }
        public class GamePlayer
    {
        public int Position { get; set; }
        public Player PlayerInfo { get; set; }

        public GamePlayer(Player playerInfo)
        {
            Position = 0;
            PlayerInfo = playerInfo;
        }
    }

    public class GameMap
    {
        public GamePlayer Player { get; set; }
        // Game state fields
        public string PlayerId { get; set; }
        public bool GameStart { get; set; }
        public int TickId { get; set; }
        public dynamic MapInfo { get; set; }
        public List<List<int>> Map { get; set; }
        public List<int> FlatMap { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Dictionary<string, GamePlayer> PlayerMap { get; set; }
        public HashSet<int> OpponentPositions { get; set; }
        public HashSet<int> Mystics { get; set; }
        public HashSet<int> Eggs { get; set; }
        public List<int> OldOpponentBombs { get; set; }
        public List<int> NewOpponentBombs { get; set; }
        public HashSet<int> BombSpots { get; set; }
        public HashSet<int> BombDangers { get; set; }
        public HashSet<int> BombPositions { get; set; }
        public Dictionary<int, int> BombMap { get; set; }
        public HashSet<int> RottenBoxes { get; set; }
        public bool GameLock { get; set; }
        public bool EndGame { get; set; }
        public List<int> RoadMap { get; set; }
        public DateTime DestinationStart { get; set; }
        public DateTime DestinationEnd { get; set; }
        public bool HaltSignal { get; set; }
        public int HaltSignalTime { get; set; }
        public HashSet<int> ReachableCells { get; set; }
        public HashSet<int> EmptyCells { get; set; }
        public TreeNode PivotNode { get; set; }
        public HashSet<int> EditableEggs { get; set; }
        public Dictionary<int, TreeNode> AllLeaves { get; set; }
        public List<TreeNode> GstTargets { get; set; }
        public TreeNode Spot1 { get; set; }
        public TreeNode Spot2 { get; set; }
        public TreeNode Spot3 { get; set; }
        public TreeNode GstEgg1 { get; set; }
        public TreeNode GstEgg2 { get; set; }
        public TreeNode GstEgg3 { get; set; }
        public bool IgnoreMystic { get; set; }
        public bool AvoidMystic { get; set; }
        public bool CanMove { get; set; }
        public bool InDanger { get; set; }
        public bool CanBomb { get; set; }
        public bool HaveEditableEggs { get; set; }
        public int GstEggBeingAttacked { get; set; }
        public int CountGoExploreSpecialSpot { get; set; }

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
            AvoidMystic = true;
            CanMove = false;
            InDanger = false;
            CanBomb = true;
            HaveEditableEggs = false;
            GstEggBeingAttacked = 0;
            CountGoExploreSpecialSpot = 0;
        }



    }
    public class TicktackResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("map_info")]
        public MapInfoDto MapInfo { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("player_id")]
        public string PlayerId { get; set; }

        [JsonProperty("gameRemainTime")]
        public int GameRemainTime { get; set; }
    }

    //public class SizeDto
    //{
    //    [JsonProperty("cols")]
    //    public int Cols { get; set; }
    //    [JsonProperty("rows")]
    //    public int Rows { get; set; }
    //}
    //public class MapInfoDto
    //{
    //    [JsonProperty("size")]
    //    public SizeDto Size { get; set; }
    //    [JsonProperty("players")]
    //    public PlayerDto[] Players { get; set; }

    //    [JsonProperty("map")]
    //    public int[,] Map { get; set; }
    //    [JsonProperty("bombs")]
    //    public BombDto[] Bombs { get; set; }
    //    [JsonProperty("spoils")]
    //    public SpoilDto[] Spoils { get; set; }
    //    [JsonProperty("weaponHammers")]
    //    public WeaponHammer[] WeaponHammers { get; set; }
    //    [JsonProperty("weaponWinds")]
    //    public WeaponWind[] WeaponWinds { get; set; }
    //    [JsonProperty("cellSize")]
    //    public int CellSize { get; set; }
    //    [JsonProperty("gameStatus")]
    //    public string GameStatus { get; set; }

    //}


    //public class WeaponHammer
    //{

    //    /// <summary>
    //    /// 				- playerId:				playerId of the hammer's owner	
    //    /// </summary>
    //    /// 
    //    [JsonProperty("playerId")]
    //    public string PlayerId { get; set; }
    //    /// <summary>
    //    /// 				- power:				blast radius after hammer hits target				
    //    /// </summary>
    //    [JsonProperty("power")]
    //    public string Power { get; set; }
    //    /// <summary>
    //    /// 				- destination:				hammer destination								
    //    /// </summary>
    //    [JsonProperty("destination")]
    //    public Position Destination { get; set; }
    //    /// <summary>
    //    /// 				- createdAt:				the moment the hammer was created		
    //    /// </summary>
    //    [JsonProperty("createdAt")]
    //    public DateTime CreateAt { get; set; }




    //}
    //public class WeaponWind
    //{

    //    public string PlayerId { get; set; }
    //    [JsonProperty("currentCol")]
    //    public string CurrentCol { get; set; }
    //    [JsonProperty("currentRow")]
    //    public string CurrentRow { get; set; }
    //    /// <summary>
    //    /// 				- destination:				hammer destination								
    //    /// </summary>
    //    [JsonProperty("destination")]
    //    public Position Destination { get; set; }
    //    /// <summary>
    //    /// 				- createdAt:				the moment the hammer was created		
    //    /// </summary>
    //    [JsonProperty("createdAt")]
    //    public DateTime CreateAt { get; set; }
    //}
    public class SpoilDto : Position
    {
        [JsonProperty("spoil_type")]
        public int SpoilType { get; set; }
    }
    public class PlayerDto
    {
        public string Id { get; set; }
        public Position CurrentPosition { get; set; }
        public Position SpawnBegin { get; set; }
        public int Score { get; set; }
        public int Lives { get; set; }
        public int TransformType { get; set; }
        public List<int> OwnerWeapon { get; set; }
        public int CurrentWeapon { get; set; }
        public bool HasTransform { get; set; }
        public int TimeToUseSpecialWeapons { get; set; }
        public bool IsStun { get; set; }
        public int Speed { get; set; }
        public int Power { get; set; }
        public int Delay { get; set; }
        public int Box { get; set; }
        public int StickyRice { get; set; }
        public int ChungCake { get; set; }
        public int NineTuskElephant { get; set; }
        public int NineSpurRooster { get; set; }
        public int NineManeHairHorse { get; set; }
        public int HolySpiritStone { get; set; }
        public int EternalBadge { get; set; }
        public int BrickWall { get; set; }

    }

    public class Position
    {
        public int Col { get; set; }
        public int Row { get; set; }
    }
    public class BombDto
    {
        [JsonProperty("row")]
        public int Row { get; set; }
        [JsonProperty("col")]
        public int Col { get; set; }
        [JsonProperty("remainTime")]
        public int RemainTime { get; set; }
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }
        [JsonProperty("power")]
        public int Power { get; set; }
        [JsonProperty("createdAt")]
        public int CreatedAt { get; set; }

    }
}
