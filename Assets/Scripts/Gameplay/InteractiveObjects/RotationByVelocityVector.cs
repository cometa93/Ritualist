using UnityEngine;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public class RotationByVelocityVector : MonoBehaviour
    {
        private bool _rotate;

        void Update()
        {
            Vector3 vel = GetComponent<Rigidbody>().velocity;
            if (_rotate)
                transform.rotation = Quaternion.LookRotation(vel);
        }

        void OnCollisionEnter(Collision other)
        {
            _rotate = false;
        }

        void OnCollisionExit(Collision other)
        {
            _rotate = true;
        }
        
    }
}