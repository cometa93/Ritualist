using System.Collections;
using DevilMind;
using UnityEngine;

namespace Fading.AI.Enemies
{
    public class BurningSoulMissle : ShootableObject
    {
        [SerializeField] private GameObject _particleToSpawn;
        [SerializeField] private float TimeForOneUnit = 0.3f;
        private const int BurningMissleDamage = 10;

        protected override Hashtable ShootItweenAnimation()
        {
            var time = Vector2.Distance(Target, transform.position) * TimeForOneUnit;
            
            return new Hashtable()
            {
                {iT.MoveTo.easetype, iTween.EaseType.easeInCubic},
                {iT.MoveTo.time, time },
            };
        }

        protected override void OnShootHitThePlayer()
        {
            base.OnShootHitThePlayer();
            GameMaster.Hero.Stats.Power -= BurningMissleDamage;
            DestroyMe();
        }

        protected override void OnShootMissed()
        {
            base.OnShootMissed();
            DestroyMe();
        }

        protected override void OnTargetReached()
        {
            base.OnTargetReached();
            DestroyMe();
        }

        private void DestroyMe()
        {
            var go = Instantiate(_particleToSpawn, transform.position, Quaternion.identity) as GameObject;
            if (go != null)
            {
                go.SetActive(true);
                Destroy(go, 2f);
            }
            else
            {
                Log.Warning(MessageGroup.Gameplay, "Burning soul missle destroy particles are not set");
            }
            Destroy(this.gameObject, 0.5f);
        }
    }
}