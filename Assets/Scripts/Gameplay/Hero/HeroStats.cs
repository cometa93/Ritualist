using System;
using DevilMind;
using EventType = DevilMind.EventType;

namespace Fading
{
    [Serializable]
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
            _power = MaxPower;
        }

        public HeroStats(HeroStats stats)
        {
            _power = stats.Power;
        }

        public HeroStats CurrentStats()
        {
            return new HeroStats(this);
        }

        public static HeroStats LoadStats()
        {
            if (GameMaster.GameSave.CurrentSave == null)
            {
                Log.Warning(MessageGroup.Common, "Current save is null so stats cannot be loaded");
                return null;
            }

            return GameMaster.GameSave.CurrentSave.HeroStats;
        }
    }
}