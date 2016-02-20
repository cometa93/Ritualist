using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ritualist
{
    public class HeroSkill
    {
        private int _currentCooldown;
        protected readonly List<float> Cooldowns = new List<float>();
        public SkillEffect Effect { protected set; get; }
        public float Cooldown
        {
            get
            {
                if (Cooldowns.Count == 0)
                {
                    return 0;
                }
                var cooldownToReturn = Cooldowns[_currentCooldown];
                _currentCooldown++;
                if (_currentCooldown >= Cooldowns.Count)
                {
                    _currentCooldown = 0;
                }
                return cooldownToReturn;
            }
        }
        public float Damage;
        public float Range;
        public float LongLife;
    }
}