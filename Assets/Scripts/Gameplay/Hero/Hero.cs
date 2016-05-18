using System.Collections.Generic;
using DevilMind;
using Fading.Skills;
using Fading.AI;

namespace Fading
{
    /// <summary>
    /// Hero object is shared between gamesaves it is responsible for setting up player skills
    /// Stats are not shared but loaded personally for each save slot;
    /// </summary>
    public class Hero
    {

        public HeroStats Stats { get { return GameMaster.GameSave.CurrentSave.HeroStats; } } 
        private SkillEffect _currentSkillEffect;
        public HeroSkill CurrentHeroSkill;
        public readonly Dictionary<SkillEffect, HeroSkill> Skills = new Dictionary<SkillEffect, HeroSkill>();

        public Hero()
        {
            Skills.Add(SkillEffect.Catch, new CatchSkill());
            Skills.Add(SkillEffect.ProtectionField, new ProtectionField());

            CurrentHeroSkill = Skills[_currentSkillEffect];
        }

        public void ChangeSkill(SkillEffect skillEffect)
        {
            if (skillEffect == _currentSkillEffect)
            {
                Log.Info(MessageGroup.Gameplay, "CurrentSkillEffect is the same as before : " + skillEffect);
                return;
            }

            if (Skills.TryGetValue(skillEffect, out CurrentHeroSkill) == false)
            {
                Log.Warning(MessageGroup.Gameplay, "There is no registered skill with effect : " + skillEffect);
                return;
            }

            Log.Info(MessageGroup.Gameplay, "Current Skill changed : " + skillEffect);
            _currentSkillEffect = skillEffect;
            GameMaster.Events.Rise(EventType.HeroSkillChanged);
        }
    }
}