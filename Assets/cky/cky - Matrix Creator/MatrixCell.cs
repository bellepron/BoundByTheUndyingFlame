using CKY_Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace cky.MatrixCreation
{
    [System.Serializable]
    public class ItemTransforms
    {
        public List<Transform> items = new List<Transform>();
    }

    public class MatrixCell : MonoBehaviour
    {
        [Space(5)]
        public MatrixCreatorManager Manager;

        [Space(5)]
        public int I;
        public int J;
        public List<MatrixCell> neighbours = new List<MatrixCell>();

        [Space(5)]
        [SerializeField] ItemTransforms[] itemTransforms_s;

        [SerializeField] ItemIndexes[] itemIndexes_s;
        bool _active;

        public void Init(MatrixCreatorManager manager, int i, int j)
        {
            Manager = manager;
            I = i;
            J = j;

            itemIndexes_s = new ItemIndexes[manager.controllers.Length];

            for (int k = 0; k < itemIndexes_s.Length; k++)
            {
                //Debug.Log($"{I} - {J}");
                if (Manager.controllers[k])
                {
                    if (Manager.controllers[k].Settings)
                    {
                        itemIndexes_s[k] = Manager.controllers[k].Settings.cells_ItemIndexes[I * Manager.Dimension_J + J];
                    }
                    else
                    {
                        Debug.LogWarning($"{Manager.controllers[k]} Settings parameter is null.");
                    }
                }
                else
                {
                    Debug.LogWarning($"{Manager.controllers[k]} is null.");
                }
            }
            itemTransforms_s = new ItemTransforms[manager.controllers.Length];
            for (int k = 0; k < itemTransforms_s.Length; k++)
            {
                itemTransforms_s[k] = new ItemTransforms();
            }
        }

        public void Open()
        {
            _active = true;
            //Debug.Log($"Opened - {I}:{J}");

            for (int k = 0; k < itemIndexes_s.Length; k++)
            {
                if (itemIndexes_s[k] != null)
                {
                    foreach (int i in itemIndexes_s[k].Indexes)
                    {
                        var controller = Manager.controllers[k];
                        var settings = controller.Settings;
                        if (controller.ItemPrefab)
                        {
                            var item = CKY_PoolManager.Spawn(controller.ItemPrefab, settings.positions[i], settings.rotations[i]);
                            if (controller.UseScale) item.localScale = settings.scales[i];

                            if (item.TryGetComponent<IMatrixItem>(out var iMatrixItem))
                            {
                                iMatrixItem.ReSpawn();
                            }
                            itemTransforms_s[k].items.Add(item);
                        }
                        else
                        {
                            Debug.LogWarning($"{controller} ItemPrefab is null.");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"{itemIndexes_s} {k}. element is null.");
                }
            }
        }

        public void Close()
        {
            _active = false;
            //Debug.Log($"Closed - {I}:{J}");

            foreach (var c in itemTransforms_s)
            {
                foreach (var item in c.items)
                {
                    CKY_PoolManager.Despawn(item);
                }
            }

            foreach (var c in itemTransforms_s)
            {
                c.items.Clear();
            }
        }



        #region Gizmos

        private void OnDrawGizmos()
        {
            Gizmos.color = _active ? Manager.colorCell_Open : Manager.colorCell_Close;
            Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, 0f, transform.localScale.z));
        }

        #endregion

    }
}