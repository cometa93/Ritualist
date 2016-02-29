using System.Collections.Generic;

namespace Ritualist.Skills
{
    public class CatchSkill : HeroSkill
    {
        public CatchSkill()
        {
            Effect = SkillEffect.Catch;
            Cooldowns.Add(0);
            Cooldowns.Add(0);
            Cooldowns.Add(3);
            LongLife = 5f;
            Range = 2f;
            PowerCost = 5;
        }
    }
}