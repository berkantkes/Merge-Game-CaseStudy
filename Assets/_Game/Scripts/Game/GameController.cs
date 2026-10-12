using MergeGame.Board;
using MergeGame.Items;
using MergeGame.Levels;
using MergeGame.Pooling;
using UnityEngine;

namespace MergeGame.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LevelDataSO levelData;
        [SerializeField] private ItemCatalogSO catalog;
        [SerializeField] private BoardView boardView;
        public BoardState State { get; private set; }

        private void Start()
        {
            if (boardView == null)
            {
                Debug.LogError("Board view is missing.");
                return;
            }
            State = BoardState.Create(levelData, catalog);
            if (State == null)
            {
                return;
            }
            var poolFactory = PoolFactory.Instance;
            poolFactory.Initialize();
            boardView.Initialize(poolFactory);
            Canvas.ForceUpdateCanvases();
            boardView.Build(levelData, State, catalog);
        }
    }
}
