using UnityEngine;

namespace DevilMind.Utils
{
    public static class TransformExtensions
    {
        public static void SetLayer(this Transform transform, GameObject go)
        {
            if (go == null)
            {
                Log.Error(MessageGroup.Common, "gameobject in set layer is null");
                return;
            }
            transform.gameObject.layer = go.layer;
        }

        public static void SetLayerRecursive(this Transform t, GameObject go)
        {
            if (t == null)
            {
                return;
            }

            t.SetLayer(go);

            foreach (Transform child in t)
            {
                child.SetLayerRecursive(go);
            }
        }

        public static void ResetLocals(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.eulerAngles = Vector3.zero;
        }
    }
}