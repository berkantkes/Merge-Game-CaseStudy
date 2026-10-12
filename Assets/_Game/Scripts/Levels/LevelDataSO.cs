using System.Collections.Generic;
using UnityEngine;

namespace MergeGame.Levels
{
    [CreateAssetMenu(fileName = "Level", menuName = "Merge Game/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField, Min(1)] private int rows = 5;
        [SerializeField, Min(1)] private int columns = 5;
        [SerializeField, Min(0)] private float rowSpacing = 6;
        [SerializeField, Min(0)] private float columnSpacing = 6;
        [SerializeField] private List<CellPlacement> placements = new List<CellPlacement>();

        public int Rows => rows;
        public int Columns => columns;
        public float RowSpacing => rowSpacing;
        public float ColumnSpacing => columnSpacing;
        public IReadOnlyList<CellPlacement> Placements => placements;
    }
}
