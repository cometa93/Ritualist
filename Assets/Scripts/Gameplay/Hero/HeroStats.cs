using UnityEngine;

namespace Ritualist
{
    public class HeroStats
    {
        public int Health = 100;
        public int MaxHealth { get ; private set; }
        
        public void Reset()
        {
            MaxHealth = 100;
            Health = 100;
        }
    }
}