using System.Collections.Generic;

namespace Ritualist.Skills
{
    public class CatchSkill : HeroSkill
    {
        public CatchSkill()
        {
            Effect = SkillEffect.Catch;
            Range = 10f;
            PowerCost = 2;
        }
    }
}