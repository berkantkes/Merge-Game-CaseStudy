using System;
using MergeGame.Board;

namespace MergeGame.Events
{
    public static class EventManager
    {
        public static Action SpawnRequested;
        public static Action MenuRequested;
        public static Action RestartRequested;
        public static Action<bool> MenuPopupChanged;
        public static Action<int> ScoreChanged;
        public static Action<BoardPointer> BoardDragStarted;
        public static Action<BoardPointer> BoardDragMoved;
        public static Action<BoardPointer> BoardDragEnded;
        public static Action BoardInteractionCancelled;
        public static Action<bool> BoardInteractionBusyChanged;
        public static Action<int, int> CellPressed;
    }
}
