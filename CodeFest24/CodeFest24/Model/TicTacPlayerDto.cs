using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFest24.Model
{
    public class TicTacPlayerDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("timestamp")]
        public string TimeStamp { get; set; }

        [JsonPropertyName("map_info")]
        public MapInfo MapInfo { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("playerId:")]
        public string PlayerId { get; set; }

        [JsonPropertyName("gameRemainTime")]
        public int GameRemainTime { get; set; }
    }

    public class MapInfo
    {
        [JsonPropertyName("size")]
        public Size Size { get; set; }

        [JsonPropertyName("players")]
        public List<Player> Players { get; set; }

        [JsonPropertyName("map")]
        public List<List<int>> Map { get; set; } // 2D array of map values

        [JsonPropertyName("bombs")]
        public List<Bomb> Bombs { get; set; }

        [JsonPropertyName("spoils")]
        public List<Spoil> Spoils { get; set; }

        [JsonPropertyName("weaponHammers")]
        public List<WeaponHammer> WeaponHammers { get; set; }

        [JsonPropertyName("weaponWinds")]
        public List<WeaponWind> WeaponWinds { get; set; }

        [JsonPropertyName("cellSize")]
        public int CellSize { get; set; }

        [JsonPropertyName("gameStatus")]
        public EGameStatus? GameStatus { get; set; }
    }

    public class Size
    {
        [JsonPropertyName("rows")]
        public int Rows { get; set; }

        [JsonPropertyName("cols")]
        public int Cols { get; set; }
    }

    public class Player
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("spawnBegin")]
        public SpawnBegin SpawnBegin { get; set; }

        [JsonPropertyName("currentPosition")]
        public CurrentPosition CurrentPosition { get; set; }

        [JsonPropertyName("power")]
        public int Power { get; set; }

        [JsonPropertyName("speed")]
        public int Speed { get; set; }

        [JsonPropertyName("delay")]
        public int Delay { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("lives")]
        public int Lives { get; set; }

        [JsonPropertyName("box")]
        public int Box { get; set; }

        [JsonPropertyName("stickyRice")]
        public int StickyRice { get; set; }

        [JsonPropertyName("chungCake")]
        public int ChungCake { get; set; }

        [JsonPropertyName("nineTuskElephant")]
        public int NineTuskElephant { get; set; }

        [JsonPropertyName("nineSpurRooster")]
        public int NineSpurRooster { get; set; }

        [JsonPropertyName("nineManeHairHorse")]
        public int NineManeHairHorse { get; set; }

        [JsonPropertyName("holySpiritStone")]
        public int HolySpiritStone { get; set; }

        [JsonPropertyName("eternalBadge")]
        public int EternalBadge { get; set; }

        [JsonPropertyName("brickWall")]
        public int BrickWall { get; set; }

        [JsonPropertyName("transformType")]
        public int TransformType { get; set; }

        [JsonPropertyName("hasTransform")]
        public bool HasTransform { get; set; }

        [JsonPropertyName("ownerWeapon")]
        public int[] OwnerWeapon { get; set; } = new int[1];

        [JsonPropertyName("curWeapon")]
        public int CurWeapon { get; set; }

        [JsonPropertyName("isStun")]
        public bool IsStun { get; set; }

        [JsonPropertyName("timeToUseSpecialWeapons")]
        public int TimeToUseSpecialWeapons { get; set; }
    }

    public class SpawnBegin
    {
        [JsonPropertyName("rows")]
        public int Rows { get; set; }

        [JsonPropertyName("cols")]
        public int Cols { get; set; }
    }

    public class CurrentPosition
    {
        [JsonPropertyName("rows")]
        public int Rows { get; set; }

        [JsonPropertyName("cols")]
        public int Cols { get; set; }
    }

    public class Bomb
    {
        [JsonPropertyName("rows")]
        public int Rows { get; set; }

        [JsonPropertyName("cols")]
        public int Cols { get; set; }

        [JsonPropertyName("remainTime")]
        public int RemainTime { get; set; }

        [JsonPropertyName("playerId:")]
        public string PlayerId { get; set; }

        [JsonPropertyName("power")]
        public int Power { get; set; }

        [JsonPropertyName("createdAt")]
        public int CreatedAt { get; set; }
    }

    public class Spoil
    {
        [JsonPropertyName("rows")]
        public int Rows { get; set; }

        [JsonPropertyName("cols")]
        public int Cols { get; set; }

        [JsonPropertyName("spoil_type")]
        public ESpoilType SpoilType { get; set; }
    }

    public class WeaponHammer
    {
        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        [JsonPropertyName("power")]
        public int Power { get; set; }

        [JsonPropertyName("destination")]
        public int Destination { get; set; }

        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }
    }

    public class WeaponWind
    {
        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        [JsonPropertyName("currentRow")]
        public int CurrentRow { get; set; }

        [JsonPropertyName("currentCol")]
        public int CurrentCol { get; set; }

        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("destination")]
        public int Destination { get; set; }
    }
}
