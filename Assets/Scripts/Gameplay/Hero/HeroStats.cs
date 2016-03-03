using DevilMind;
using EventType = DevilMind.EventType;

namespace Ritualist
{
    public class HeroStats
    {
        private int _power;
        public int Power
        {
            set
            {
                _power = value;
                if (_power > MaxPower)
                {
                    _power = MaxPower;
                }
                GameMaster.Events.Rise(EventType.HeroPowerChanged);
            }
            get { return _power; }   
        }

        public int MaxPower
        {
            get { return 100; }
        }
        


        public HeroStats()
        {
            _power = 30;
        }
    }
}