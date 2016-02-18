using DevilMind;
using UnityEngine;

namespace Ritualist.Controller
{
    public class AimBehaviour : DevilBehaviour
    {
        public void Rotate(Vector2 temp)
        {
            var dir = transform.localPosition - new Vector3(temp.x, temp.y, 0);
            dir.Normalize();

            var rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0, 0, rotation);
        }
    }
}