using UnityEngine;

namespace cky.MatrixCreation
{
    public class MatrixCreatorManager : MonoBehaviour
    {
        [field: SerializeField] public Transform PlayerTransform { get; set; }
        [field: SerializeField] public MatrixCell PlayerCell_Previous { get; set; }
        [field: SerializeField] public MatrixCell PlayerCell_Current { get; set; }

        [field: SerializeField] public GameObject MatrixCreatorGO { get; set; }
        [field: SerializeField] public MatrixCreator MatrixCreator { get; set; }
        [field: SerializeField] public GameObject MatrixCellPrefab { get; set; }
        [field: SerializeField] public float MatrixCellPercent { get; set; } = 0.98f;
        [field: SerializeField] public float AreaWidth_I { get; set; } = 4000f;
        [field: SerializeField] public float AreaWidth_J { get; set; } = 10000f;
        [field: SerializeField] public int Dimension_I { get; set; } = 20;
        [field: SerializeField] public int Dimension_J { get; set; } = 20;
        [field: SerializeField] public MatrixCell[,] Matrix { get; set; }

        [Space(15)]
        [Header("Execution")]
        [SerializeField] private float executionFrequency = 0.2f;

        public MatrixCreatorController[] controllers;

        public void Initialize(Transform playerTransform)
        {
            AwakeFunctions();

            PlayerTransform = playerTransform;

            MatrixCreator.Find_PlayerCell();

            InvokeRepeating(nameof(Toggle), 0, executionFrequency);
        }

        private void AwakeFunctions()
        {
            CreateMatrixCreator();
            MatrixCreator.Create(this);
        }

        private void Toggle()
        {
            MatrixCreator.ToggleCellAndNeighbours();
        }



        #region Editor Matrix Set

        public void MatrixSet()
        {
            CreateMatrixCreator();

            //foreach (var controller in controllers)
            //{
            //    controller.GetItems(Dimension_I, Dimension_J);
            //}

            MatrixCreator.Create(this);
        }
        private void CreateMatrixCreator()
        {
            for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
            MatrixCreatorGO = new GameObject($"Matrix Creator");
            MatrixCreatorGO.transform.SetParent(transform, false);
            MatrixCreator = MatrixCreatorGO.AddComponent<MatrixCreator>();
        }

        #endregion



        #region Gizmos

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 center = transform.position;
            Gizmos.DrawWireCube(center, new Vector3(AreaWidth_J, -0.01f, AreaWidth_I));
        }

        #endregion
    }
}