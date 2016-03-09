using UnityEngine;

namespace DevilMind
{
    public static class DevilMath
    {
        public static float GetAngleBeetwenPoints(float x, float y)
        {
            return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        }

    }
}