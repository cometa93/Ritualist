using UnityEngine;

namespace DevilMind
{
    public static class DevilMath
    {
        public static float GetAngleBeetwenPoints(float x, float y)
        {
            return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// angle > 0 will rotate vector opposite to clock direction
        /// angle greater 0 will rotate vector in clock direction
        /// </summary>
        /// <returns></returns>
        public static Vector3 RotateVectorZByAngle(Vector3 vectorToRotate, float angle)
        {
            var x = vectorToRotate.x;
            var y = vectorToRotate.y;
            angle = angle*Mathf.Deg2Rad;
            var tmpX = x*Mathf.Cos(angle) - y*Mathf.Sin(angle);
            var tmpY = x*Mathf.Sin(angle) + y*Mathf.Cos(angle);
            vectorToRotate.x = tmpX;
            vectorToRotate.y = tmpY;
            return vectorToRotate;
        }
    }
}