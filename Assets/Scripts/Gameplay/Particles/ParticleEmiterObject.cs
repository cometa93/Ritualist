using UnityEngine;

namespace Assets.Scripts.Gameplay.Particles
{
    public class ParticleEmiterObject : MonoBehaviour
    {
        [SerializeField] private float _untilDie;

        private void Update()
        {
            _untilDie -= Time.deltaTime;
            if (_untilDie < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}