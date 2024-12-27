using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace cky.MatrixCreation
{
    //[System.Serializable]
    //public class ItemDataBLABLA
    //{
    //    public Vector3 position;
    //    public Quaternion rotation;
    //    public Vector3 scale;
    //    public List<int> Indexes = new List<int>();
    //}

    [System.Serializable]
    public class MatrixItemData
    {
        public MatrixSettings Settings;
        public Transform ItemPrefab;
        public bool UseScale;
        public bool WillCreate;
        MatrixItemTypes _matrixItemType;
        Transform[] _items;

        public void GetItems(int Dimension_I, int Dimension_J, IMatrixItem[] allMatrixItems, MatrixCreator matrixCreator)
        {
            Settings.Dimension_I = Dimension_I;
            Settings.Dimension_J = Dimension_J;

            _matrixItemType = ItemPrefab.GetComponent<IMatrixItem>().MatrixItemType;
            _items = allMatrixItems.Where(i => i.MatrixItemType == _matrixItemType).Select(i => i.Transform).ToArray();
            var itemCount = _items.Length;
            Debug.Log($"MatrixItemType - {_matrixItemType} count: {itemCount}");

            Settings.positions = new Vector3[itemCount];
            Settings.rotations = new Quaternion[itemCount];
            if (UseScale) Settings.scales = new Vector3[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                Settings.positions[i] = _items[i].position;
                Settings.rotations[i] = _items[i].rotation;
                if (UseScale) Settings.scales[i] = _items[i].localScale;
            }

            Settings.cells_ItemIndexes = new ItemIndexes[Dimension_I * Dimension_J];
            for (int i = 0; i < Dimension_I * Dimension_J; i++)
            {
                Settings.cells_ItemIndexes[i] = new ItemIndexes();
            }

            matrixCreator.AssignItemsToCellsTo_ScriptableObject(this);

#if UNITY_EDITOR
            EditorUtility.SetDirty(Settings);
#endif
        }
    }
}