using UnityEngine;

namespace MergeGame.Board
{
    public interface IBoardDropRules
    {
        DropResult Evaluate(BoardState state, Vector2Int source, Vector2Int? target);
    }
}
