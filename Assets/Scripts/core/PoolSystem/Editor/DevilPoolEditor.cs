
using UnityEditor;

namespace DevilMind
{
    [CustomEditor(typeof(DevilPool))]
    public class DevilPoolEditor : Editor
    {
        private SerializedProperty _poolName;
        private DevilPool poolToSerialize;

        public void OnEnable()
        {
            _poolName = serializedObject.FindProperty("_poolName");
            if (poolToSerialize == null)
            {
                poolToSerialize = (DevilPool) target;
            }
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_poolName);
            EditorGUILayout.Separator();
       //     poolToSerialize.Inspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

