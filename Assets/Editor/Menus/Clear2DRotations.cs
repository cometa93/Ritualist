using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ritualist
{
    public class Clear2DRotations : ScriptableWizard
    {

        [MenuItem("Devilmind/Transforms/Clear 2D Rotations")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "Is Clearing X and Y rotation of transform for 2D", typeof(Clear2DRotations), "Clear Rotations");
        }

        void OnWizardCreate()
        {
            Transform[] transforms = Selection.GetTransforms(SelectionMode.DeepAssets | SelectionMode.Unfiltered | SelectionMode.Deep);
            List<Transform> transformsToSort = new List<Transform>(transforms);

            for (int i = 0, c = transformsToSort.Count; i < c; ++i)
            {
                Transform t = transformsToSort[i];
                t.localRotation = Quaternion.Euler(new Vector3(0, 0, t.localRotation.z));
            }
        }

    }
}