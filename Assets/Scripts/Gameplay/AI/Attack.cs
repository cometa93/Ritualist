namespace Ritualist.AI
{
    public enum AttackEffect
    {
        None,
        Freeze,
        Poison,
        Stunt,
    }

    public abstract class Attack
    {
        public float Range;
        public float Cooldown;
        public float Damage;
        public AttackEffect Effect;
        public float EffectValue;
    }
}