using UnityEditor;
using UnityEngine;

namespace Fading
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
            foreach (Transform t in Selection.activeTransform)
            {
                SetOrderLayer(t);
            }
        }

        private void SetOrderLayer(Transform t)
        {
            Renderer renderer = t.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = (int)(-t.position.z * 100f);
                Debug.Log(t.name + " sorting order changed");
            }

            foreach (Transform child in t)
            {
                SetOrderLayer(child);
            }
        }
    }
}