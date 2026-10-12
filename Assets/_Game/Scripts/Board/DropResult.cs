namespace MergeGame.Board
{
    public enum DropResult
    {
        Move,
        SameCell,
        Merge,
        DifferentType,
        DifferentLevel,
        MaximumLevel,
        OutsideBoard,
        EmptySource
    }
}
