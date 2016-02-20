using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
using UnityEngine;

namespace Ritualist.AI
{
    public abstract class AbstractEnemy : DevilBehaviour
    {
        [SerializeField] protected Transform Player;

        public EnemyType EnemyType;
        public string Name;
        public int Health;
        public int ProteciveField;
        protected bool IsAlive;
        protected Attack NormalAttack;

        protected readonly List<Attack> SpecialAttacks = new List<Attack>();
        protected readonly List<Defence> Defences = new List<Defence>();

        protected override void Awake()
        {
            Setup();
            base.Awake();
        }

        protected override void Update()
        {
            Move();
            Attack();
        }

        protected virtual void FixedUpdate()
        {
            LookoutFortrap();
        }

        protected abstract void Setup();

        protected abstract void Move();

        protected abstract void Attack();

        protected abstract void LookoutFortrap();

    }
}