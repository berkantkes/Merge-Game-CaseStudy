using MergeGame.Board;
using MergeGame.Events;
using MergeGame.Items;
using MergeGame.Levels;
using MergeGame.Loading;
using MergeGame.Pooling;
using UnityEngine;

namespace MergeGame.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LevelDataSO levelData;
        [SerializeField] private ItemCatalogSO catalog;
        [SerializeField] private BoardView boardView;
        [SerializeField] private GameView gameView;
        [SerializeField] private ItemDragController itemDragController;
        private ItemSpawner itemSpawner;
        private bool interactionBusy;
        private bool menuPopupOpen;
        private int pendingSpawns;
        public BoardState State { get; private set; }

        private void OnEnable()
        {
            EventManager.SpawnRequested += SpawnItem;
            EventManager.MenuRequested += ReturnToMenu;
            EventManager.RestartRequested += RestartLevel;
            EventManager.MenuPopupChanged += SetMenuPopupOpen;
            EventManager.BoardInteractionBusyChanged += SetInteractionBusy;
        }

        private void OnDisable()
        {
            EventManager.SpawnRequested -= SpawnItem;
            EventManager.MenuRequested -= ReturnToMenu;
            EventManager.RestartRequested -= RestartLevel;
            EventManager.MenuPopupChanged -= SetMenuPopupOpen;
            EventManager.BoardInteractionBusyChanged -= SetInteractionBusy;
        }

        private void Start()
        {
            if (boardView == null || gameView == null)
            {
                Debug.LogError("Game references are missing.");
                return;
            }
            var dropRules = new BoardDropRules(catalog);
            State = BoardState.Create(levelData, catalog, dropRules);
            if (State == null)
            {
                return;
            }
            var poolFactory = PoolFactory.Instance;
            poolFactory.Initialize();
            boardView.Initialize(poolFactory);
            Canvas.ForceUpdateCanvases();
            bool built = boardView.Build(levelData, State, catalog);
            if (!built)
            {
                return;
            }
            var spawnPolicy = new RandomItemSpawnPolicy(catalog);
            itemSpawner = new ItemSpawner(State, catalog, spawnPolicy);
            var scoreCalculator = new MergeScoreCalculator(catalog);
            var boardActions = new BoardActions(State, scoreCalculator);
            itemDragController.Initialize(boardActions, catalog, poolFactory);
            gameView.ShowScore(boardActions.Score);
            boardView.SetInputEnabled(true);
            RefreshSpawnButton();
        }

        private void ReturnToMenu()
        {
            SceneLoader.Load(Constants.SceneNames.Menu);
        }

        private void RestartLevel()
        {
            SceneLoader.Load(Constants.SceneNames.Game);
        }

        private void SetMenuPopupOpen(bool open)
        {
            menuPopupOpen = open;
            boardView.SetInputEnabled(!open && pendingSpawns == 0);
            RefreshSpawnButton();
        }

        private void SpawnItem()
        {
            if (itemSpawner == null || interactionBusy || menuPopupOpen)
            {
                return;
            }
            var position = itemSpawner.TrySpawn();
            if (position.HasValue)
            {
                pendingSpawns++;
                boardView.SetInputEnabled(false);
                int row = position.Value.y;
                int column = position.Value.x;
                var item = State.GetCell(row, column);
                catalog.TryGet(item.Type, item.Level, out var definition);
                var target = boardView.GetItemTransform(row, column);
                gameView.PlaySpawn(definition.Sprite, target, () =>
                {
                    boardView.PlayItemArrival(row, column);
                    pendingSpawns--;
                    if (pendingSpawns == 0 && !menuPopupOpen)
                    {
                        boardView.SetInputEnabled(true);
                    }
                });
            }
            RefreshSpawnButton();
        }

        private void RefreshSpawnButton()
        {
            bool hasEmptyCell = State.HasEmptyCell();
            gameView.SetSpawnEnabled(hasEmptyCell && !interactionBusy && !menuPopupOpen);
        }

        private void SetInteractionBusy(bool busy)
        {
            interactionBusy = busy;
            boardView.SetInteractionLocked(busy);
            if (State != null)
            {
                RefreshSpawnButton();
            }
        }
    }
}
