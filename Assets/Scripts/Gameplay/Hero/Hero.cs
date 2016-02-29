using System.Collections.Generic;
using Ritualist.AI;
using Ritualist.Skills;

namespace Ritualist
{
    public class Hero
    {
        public readonly HeroStats Stats = new HeroStats();
        public SkillEffect CurrentSkillEffect;
        public HeroSkill CurrentHeroSkill;
        public readonly Dictionary<SkillEffect, HeroSkill> Skills = new Dictionary<SkillEffect, HeroSkill>();

        public Hero()
        {
            Skills.Add(SkillEffect.Catch, new CatchSkill());
            CurrentHeroSkill = Skills[CurrentSkillEffect];
        }
    }
}