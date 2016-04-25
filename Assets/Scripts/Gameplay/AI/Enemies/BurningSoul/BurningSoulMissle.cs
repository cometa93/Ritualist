using System.Collections;
using DevilMind;
using UnityEngine;

namespace Ritualist.AI.Enemies
{
    public class BurningSoulMissle : ShootableObject
    {
        private const float TimeForOneUnit = 1f;
        private const int BurningMissleDamage = 10;

        protected override Hashtable ShootItweenAnimation()
        {
            var time = Vector2.Distance(Target, transform.position) * TimeForOneUnit;
            
            return new Hashtable()
            {
                {iT.MoveTo.easetype, EaseType.easeInQuart },
                {iT.MoveTo.time, time },
            };
        }

        protected override void OnShootHitThePlayer()
        {
            base.OnShootHitThePlayer();
            GameMaster.Hero.Stats.Power -= BurningMissleDamage;
        }
    }
}