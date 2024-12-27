using UnityEngine;

namespace cky.MatrixCreation
{
    public class MatrixCreatorController : MonoBehaviour
    {
        //[field: SerializeField] public MatrixCreator MatrixCreator { get; set; }

        [field: SerializeField] public MatrixSettings Settings { get; private set; }
        [field: SerializeField] public Transform ItemPrefab { get; private set; }
        [HideInInspector] public string ItemTag;
        [HideInInspector] public GameObject[] Items;
        [HideInInspector] public bool UseScale = false;
        [HideInInspector] public bool IsSettingsReady = false;



        #region Editor Set

        public void GetItems(int Dimension_I, int Dimension_J)
        {
            Settings.Dimension_I = Dimension_I;
            Settings.Dimension_J = Dimension_J;

            if (!IsSettingsReady)
            {
                Items = GameObject.FindGameObjectsWithTag(ItemTag);
                var itemCount = Items.Length;

                Settings.positions = new Vector3[itemCount];
                Settings.rotations = new Quaternion[itemCount];
                if (UseScale) Settings.scales = new Vector3[itemCount];
                for (int i = 0; i < itemCount; i++)
                {
                    Settings.positions[i] = Items[i].transform.position;
                    Settings.rotations[i] = Items[i].transform.rotation;
                    if (UseScale) Settings.scales[i] = Items[i].transform.localScale;
                }
            }

            Settings.cells_ItemIndexes = new ItemIndexes[Dimension_I * Dimension_J];
            for (int i = 0; i < Dimension_I * Dimension_J; i++)
            {
                Settings.cells_ItemIndexes[i] = new ItemIndexes();
            }

            FindFirstObjectByType<MatrixCreator>().AssignItemsToCellsTo_ScriptableObject(this, FindFirstObjectByType<MatrixCreatorManager>());
        }

        #endregion

    }
}