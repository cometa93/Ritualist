using UnityEditor;
using UnityEngine;

namespace DevilMind.EditorGUI
{
    [CustomEditor(typeof(DevilBehaviour),true)]
    public class DevilBehaviourEditor : Editor
    {
        private DevilBehaviour _targetBehaviour;

        public override void OnInspectorGUI()
        {

            _targetBehaviour = target as DevilBehaviour;
            if (_targetBehaviour == null)
            {
                Debug.LogWarning("target behaviour in devilbehaviour editor is null");
                base.OnInspectorGUI();
                return;
            }
            if (GUILayout.Button("GENERATE UNIQUE ID"))
            {
                
            }
            base.OnInspectorGUI();
        }

        private void CheckAndGenerateUniqueID()
        {
            
        }
    }
}