using UnityEditor;
using UnityEngine;

namespace Ritualist
{
    public class SpriteAutoLayerOrder : ScriptableWizard
    {
        [MenuItem("Devilmind/Sprite Renderers/Set Order Layers")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "SetOrderLayer", typeof(SpriteAutoLayerOrder), "Replace");
        }

        void OnWizardCreate()
        {
            Transform[] transforms = Selection.GetTransforms(SelectionMode.DeepAssets | SelectionMode.Unfiltered | SelectionMode.Deep);

            foreach (Transform t in transforms)
            {
                SpriteRenderer renderer = t.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = (int)(-t.position.z * 100f);
                }
            }
        }
    }
}