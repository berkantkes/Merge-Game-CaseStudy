using UnityEngine;

namespace MergeGame.Board
{
    public struct BoardPointer
    {
        public Vector2Int? Cell { get; }
        public Vector3 WorldPosition { get; }

        public BoardPointer(Vector2Int? cell, Vector3 worldPosition)
        {
            Cell = cell;
            WorldPosition = worldPosition;
        }
    }
}
