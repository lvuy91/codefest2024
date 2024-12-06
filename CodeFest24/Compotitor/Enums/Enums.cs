namespace Compotitor.Enums
{
    public enum MapCell
    {
        EmptyCell,// - Empty Cell (Can move through them)								
        Wall,// (None destructible cell)								
        Balk, // (Destructible cell)								
        BrickWall, // (Destructible cell by wooden pestle weapon)								
        PrisonPlace,//								
        GodBadge,// (used to turn the character into a God)								
        CellsDestroyed// by special weapons (Can move through them)								

    }
}
