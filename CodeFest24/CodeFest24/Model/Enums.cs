using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFest24
{
    public enum EMap
    {
        EmptyCell = 0,
        AWall = 1,
        ABalk = 1,
        ABrickWall = 3,
        APrisonPlace = 5,
        GodBadge = 6,
        CellsDestroyedBySpecialWeapons = 7,
    }

    public enum ESpoilType
    {
        StickyRice = 32,
        ChungCake = 33,
        NineTuskElephant = 34,
        NineSpurRooster = 35,
        NineManeHairHorse = 36,
        GHolySpiritStoneodBadge = 37,
    }

    public enum ECellSize
    {
        Training = 35,
        Fighting = 55,
    }

    public enum EGameStatus
    {
        RunningRound = 2,
        PauseRound = 3,
        GameOver = 10,
    }
}