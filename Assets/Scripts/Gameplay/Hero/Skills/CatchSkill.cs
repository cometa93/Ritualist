using System.Collections.Generic;

namespace Fading.Skills
{
    public class CatchSkill : HeroSkill
    {
        public CatchSkill()
        {
            Effect = SkillEffect.Catch;
            Range = 10f;
            PowerCost = 2;
            Damage = 10;
        }
    }
}