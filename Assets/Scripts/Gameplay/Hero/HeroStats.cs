using DevilMind;
using EventType = DevilMind.EventType;

namespace Fading
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

                if (_power <= 0)
                {
                    GameMaster.Events.Rise(EventType.CharacterDied);
                }
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