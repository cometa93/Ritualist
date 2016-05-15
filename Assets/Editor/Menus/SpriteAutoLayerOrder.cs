using UnityEditor;
using UnityEngine;

namespace Fading
{
    public class SpriteAutoLayerOrder : ScriptableWizard
    {
        public static bool _isGroundLayerSelected;

        public bool IsGroundLayerSelected;

        [MenuItem("Devilmind/Sprite Renderers/Set Order Layers")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "SetOrderLayer", typeof(SpriteAutoLayerOrder), "Replace");
        }

        void OnWizardCreate()
        {
            if (IsGroundLayerSelected)
            {
                SetGroundOrderLayer(Selection.activeTransform);
                return;
            }

            SetOrderLayer(Selection.activeTransform);
        }

        public SpriteAutoLayerOrder()
        {
            IsGroundLayerSelected = _isGroundLayerSelected;
        }

        public void OnWizardUpdate()
        {
            _isGroundLayerSelected = IsGroundLayerSelected;
        }

        private void SetOrderLayer(Transform t)
        {
            Renderer renderer = t.GetComponent<Renderer>();
            if (renderer != null)
            {
                var calculatedPosition = (int)-t.position.z;
                calculatedPosition *= 100;
                int half = (int) (10 * -t.position.z);
                var finalRenderOrdeer = half + calculatedPosition;
                renderer.sortingOrder = finalRenderOrdeer;
                Debug.Log(t.name + " : position = " + t.position + " , calculated order: " + renderer.sortingOrder);
            }

            foreach (Transform child in t)
            {
                SetOrderLayer(child);
            }
        }

        private void SetGroundOrderLayer(Transform t)
        {
            Renderer renderer = t.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = (int)(-t.position.z * 1000f);
            }

            foreach (Transform child in t)
            {
                SetGroundOrderLayer(child);
            }
        }
    }
}