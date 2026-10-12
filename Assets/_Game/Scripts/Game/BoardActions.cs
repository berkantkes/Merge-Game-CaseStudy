using MergeGame.Board;
using UnityEngine;

namespace MergeGame.Game
{
    public class BoardActions
    {
        private readonly IMergeScoreCalculator scoreCalculator;

        public BoardState Board { get; }
        public int Score { get; private set; }

        public BoardActions(BoardState board, IMergeScoreCalculator scoreCalculator)
        {
            Board = board;
            this.scoreCalculator = scoreCalculator;
        }

        public DropResult TryDrop(Vector2Int source, Vector2Int? target)
        {
            var result = Board.TryDrop(source, target);
            if (result == DropResult.Merge)
            {
                AddMergeScore(target.Value);
            }
            return result;
        }

        private void AddMergeScore(Vector2Int position)
        {
            var item = Board.GetCell(position.y, position.x);
            Score += scoreCalculator.Calculate(item);
        }
    }
}
