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

        /// <summary>
        /// Sets parent transform parent and by defualt setting layer 
        /// </summary>
        /// <param name="transform"> this transform</param>
        /// <param name="parentTransform"> parent to set </param>
        /// <param name="dontChangeTransformSetup"> if true will not change local scale, local euler angles, and local position to zero</param>
        public static void SetParent(
            this Transform transform, 
            Transform parentTransform,
            bool dontChangeTransformSetup = false)
        {
            transform.parent = parentTransform;
            if (dontChangeTransformSetup == false)
            {
                return;
            }

            transform.localScale = Vector3.one;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }



    }
}