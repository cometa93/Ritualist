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
                Renderer renderer = t.GetComponent<Renderer>();
                if (renderer  == null)
                {
                    continue;
                }
                renderer.sortingOrder = (int)(-t.position.z * 100f);
                Debug.Log(t.name + " sorting order changed");
            }
        }
    }
}