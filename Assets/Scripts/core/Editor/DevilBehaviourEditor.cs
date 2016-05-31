using System;
using UnityEditor;
using UnityEngine;

namespace DevilMind.EditorGUI
{
    [CustomEditor(typeof(DevilBehaviour),true)]
    public class DevilBehaviourEditor : Editor
    {
        private SerializedProperty _uniqueIdProperty;
        private DevilBehaviour _targetBehaviour;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {

            if (_uniqueIdProperty == null)
            {
                _uniqueIdProperty = serializedObject.FindProperty("_uniqueID");
            }

            if (_uniqueIdProperty != null)
            {
                GUILayout.BeginVertical();
                _targetBehaviour = target as DevilBehaviour;
                if (_targetBehaviour == null)
                {
                    Debug.LogWarning("target behaviour in devilbehaviour editor is null");
                    base.OnInspectorGUI();
                    return;
                }
                if (GUILayout.Button("GENERATE UNIQUE ID"))
                {
                    CheckAndGenerateUniqueID();
                }

                GUILayout.Label(string.IsNullOrEmpty(_uniqueIdProperty.stringValue)
                    ? " ---- not created ----"
                    : _uniqueIdProperty.stringValue);

                GUILayout.EndVertical();
                GUILayout.Space(15);
            }

            base.OnInspectorGUI();
        }

        private void CheckAndGenerateUniqueID()
        {
            if (_uniqueIdProperty == null)
            {
                Debug.LogWarning("Cant find unique id property for that object !");
                return;
            }

            Guid guid = Guid.NewGuid();
            _uniqueIdProperty.stringValue = guid.ToString();
            serializedObject.ApplyModifiedProperties();
        }
    }
}