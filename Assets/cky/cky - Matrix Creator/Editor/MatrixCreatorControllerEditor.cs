using UnityEditor;
using UnityEngine;

namespace cky.MatrixCreation
{
    [CustomEditor(typeof(MatrixCreatorController))]
    public class MatrixCreatorControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MatrixCreatorController mcc = (MatrixCreatorController)target;

            EditorGUILayout.Space(15);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseScale"));
            EditorGUILayout.Space(15);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsSettingsReady"));

            if (!mcc.IsSettingsReady)
            {
                EditorGUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemTag"));
                if (GUILayout.Button("Get Items"))
                {
                    var mcm = FindFirstObjectByType<MatrixCreatorManager>();
                    mcc.GetItems(mcm.Dimension_I, mcm.Dimension_J);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items"));
            }
            else
            {
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Assign Items To Cells"))
                {
                    var mcm = FindFirstObjectByType<MatrixCreatorManager>();
                    mcc.GetItems(mcm.Dimension_I, mcm.Dimension_J);
                }
            }



            serializedObject.ApplyModifiedProperties();
        }
    }
}