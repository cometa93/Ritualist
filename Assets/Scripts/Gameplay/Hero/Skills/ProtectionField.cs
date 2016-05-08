namespace Fading.Skills
{
    public class ProtectionField : HeroSkill
    {
        public ProtectionField()
        {
            Effect = SkillEffect.ProtectionField;
            Range = 6f;
            PowerCost = 1;
        }
    }
}