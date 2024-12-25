using System.Collections.Generic;
using UnityEngine;

namespace cky.MatrixCreation
{
    [System.Serializable]
    public class ItemIndexes
    {
        public List<int> Indexes = new List<int>();
    }
    public class MatrixCreator : MonoBehaviour
    {
        [System.Serializable]
        public class MatrixIndex
        {
            public int I;
            public int J;

            public MatrixIndex(int i, int j)
            {
                I = i;
                J = j;
            }
        }



        [SerializeField] MatrixCreatorManager _m;
        [SerializeField] List<MatrixCell> _openCells_Previous = new List<MatrixCell>();
        [SerializeField] List<MatrixCell> _openCells_Current = new List<MatrixCell>();



        public void Create(MatrixCreatorManager imc)
        {
            _m = imc;

            ClearMatrix();
            CreateMatrix();
            AssignCellsNeighbours();
        }

        private void ClearMatrix()
        {
            var cells = transform.GetComponentsInChildren<MatrixCell>();
            foreach (var cell in cells)
            {
                DestroyImmediate(cell.gameObject);
            }
        }

        private void CreateMatrix()
        {
            _m.Matrix = new MatrixCell[_m.Dimension_I, _m.Dimension_J];

            for (int i = 0; i < _m.Dimension_I; i++)
            {
                for (int j = 0; j < _m.Dimension_J; j++)
                {
                    GameObject cellGO = Instantiate(_m.MatrixCellPrefab, transform);
                    cellGO.transform.localPosition = new Vector3(
                        -_m.AreaWidth_J * 0.5f + _m.AreaWidth_J * (j + 0.5f) / _m.Dimension_J,
                        0.05f,
                        -_m.AreaWidth_I * 0.5f + _m.AreaWidth_I * (i + 0.5f) / _m.Dimension_I
                    );
                    cellGO.transform.localScale = new Vector3(
                        _m.AreaWidth_J / _m.Dimension_J * _m.MatrixCellPercent,
                        0.1f,
                        _m.AreaWidth_I / _m.Dimension_I * _m.MatrixCellPercent
                    );
                    cellGO.name = $"Cell [{i},{j}]";
                    MatrixCell cell = cellGO.GetComponent<MatrixCell>();
                    cell.Init(_m, i, j);
                    _m.Matrix[i, j] = cell;
                }
            }
        }

        private void AssignCellsNeighbours()
        {
            for (int i = 0; i < _m.Dimension_I; i++)
            {
                for (int j = 0; j < _m.Dimension_J; j++)
                {
                    MatrixCell currentElement = _m.Matrix[i, j];

                    if (i < _m.Dimension_I - 1 && j > 0) // Sol üst komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i + 1, j - 1]);
                    }
                    if (i < _m.Dimension_I - 1) // Üst komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i + 1, j]);
                    }
                    if (i < _m.Dimension_I - 1 && j < _m.Dimension_J - 1) // Sað üst komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i + 1, j + 1]);
                    }
                    if (j < _m.Dimension_J - 1) // Sað komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i, j + 1]);
                    }
                    if (i > 0 && j < _m.Dimension_J - 1) // Sað alt komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i - 1, j + 1]);
                    }
                    if (i > 0) // Alt komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i - 1, j]);
                    }
                    if (i > 0 && j > 0) // Sol alt komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i - 1, j - 1]);
                    }
                    if (j > 0) // Sol komþu
                    {
                        currentElement.neighbours.Add(_m.Matrix[i, j - 1]);
                    }
                }
            }
        }

        public void AssignItemsToCellsTo_ScriptableObject(MatrixCreatorController mcc, MatrixCreatorManager mcm)
        {
            _m = mcm;
            var itemPositions = mcc.Settings.positions;
            var itemsCount = mcc.Settings.positions.Length;
            for (int i = 0; i < itemsCount; i++)
            {
                var indices = Find_XZ_WithPosition(itemPositions[i]);

                if (indices.I >= 0 && indices.I < _m.Dimension_I && indices.J >= 0 && indices.J < _m.Dimension_J)
                {
                    var currentCellItemIndexes = mcc.Settings.cells_ItemIndexes[indices.I * mcc.Settings.Dimension_J + indices.J];
                    currentCellItemIndexes.Indexes.Add(i);
                }
            }
        }

        private MatrixIndex Find_XZ(Transform itemTr)
        {
            // Objelerin dünya konumunu deðil, yerel konumunu kullanarak hücreye yerleþtir
            Vector3 localPosition = transform.InverseTransformPoint(itemTr.position);
            int iIndex = Mathf.FloorToInt((localPosition.z + _m.AreaWidth_I * 0.5f) / _m.AreaWidth_I * _m.Dimension_I);
            int jIndex = Mathf.FloorToInt((localPosition.x + _m.AreaWidth_J * 0.5f) / _m.AreaWidth_J * _m.Dimension_J);

            return new MatrixIndex(iIndex, jIndex);
        }

        private MatrixIndex Find_XZ_WithPosition(Vector3 itemPosition)
        {
            // Objelerin dünya konumunu deðil, yerel konumunu kullanarak hücreye yerleþtir
            Vector3 localPosition = transform.InverseTransformPoint(itemPosition);
            int iIndex = Mathf.FloorToInt((localPosition.z + _m.AreaWidth_I * 0.5f) / _m.AreaWidth_I * _m.Dimension_I);
            int jIndex = Mathf.FloorToInt((localPosition.x + _m.AreaWidth_J * 0.5f) / _m.AreaWidth_J * _m.Dimension_J);

            return new MatrixIndex(iIndex, jIndex);
        }


        public void ToggleCellAndNeighbours()
        {
            Find_PlayerCell();

            if (_m.PlayerCell_Current != _m.PlayerCell_Previous)
            {
                _openCells_Current.Clear();

                _openCells_Current.Add(_m.PlayerCell_Current);
                foreach (var cll in _m.PlayerCell_Current.neighbours)
                {
                    _openCells_Current.Add(cll);
                }

                foreach (var cll in _openCells_Current)
                {
                    if (!_openCells_Previous.Contains(cll))
                    {
                        _m.Matrix[cll.I, cll.J].Open();
                    }
                }
                foreach (var cll in _openCells_Previous)
                {
                    if (!_openCells_Current.Contains(cll))
                    {
                        _m.Matrix[cll.I, cll.J].Close();
                    }
                }

                _m.PlayerCell_Previous = _m.PlayerCell_Current;
                _openCells_Previous = new List<MatrixCell>(_openCells_Current);
            }
        }



        public void Find_PlayerCell()
        {
            // Dünya konumunu MatrixCreator'ýn yerel konumuna dönüþtür.
            var localPosition = transform.InverseTransformPoint(_m.PlayerTransform.position);
            var pCell_I = Mathf.FloorToInt((localPosition.z + _m.AreaWidth_I * 0.5f) / _m.AreaWidth_I * _m.Dimension_I);
            var pCell_J = Mathf.FloorToInt((localPosition.x + _m.AreaWidth_J * 0.5f) / _m.AreaWidth_J * _m.Dimension_J);

            if (pCell_I >= 0 && pCell_I < _m.Dimension_I && pCell_J >= 0 && pCell_J < _m.Dimension_J)
            {
                _m.PlayerCell_Current = _m.Matrix[pCell_I, pCell_J];
            }

            //Debug.Log($"i:{_imc.PlayerCell_I} - j:{_imc.PlayerCell_J}");
        }

    }
}