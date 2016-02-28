using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ritualist
{
    public class SetZposition : ScriptableWizard
    {
        [MenuItem("Devilmind/Transforms/Set local z position")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "Sets Z Local Position Automatically", typeof(SetZposition), "Set Z");
        }

        void OnWizardCreate()
        {
            Transform[] transforms = Selection.GetTransforms(SelectionMode.DeepAssets | SelectionMode.Unfiltered | SelectionMode.Deep);
            List<Transform> transformsToSort = new List<Transform>(transforms);
            
            transformsToSort.Sort((transform, transform1) =>
            {
                return transform.localPosition.z >= transform1.localPosition.z ? 1: -1;
            });

            for (int i =0,c = transformsToSort.Count; i < c; ++i)
            {
                Transform t = transformsToSort[i];
                t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, (float)i/100);   
            }
        }
    }
}