using System.Collections.Generic;
using Assets.Scripts.Gameplay.InteractiveObjects;
using DevilMind;
using DevilMind.Utils;
using UnityEngine;

namespace Ritualist.AI
{
    public abstract class AbstractEnemy : SkillTarget
    {
        protected Transform Player;

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
        }

        protected virtual void Setup()
        {
            var playerGo = GameObject.FindGameObjectWithTag("Character");
            if (playerGo == null)
            {
                Log.Error(MessageGroup.Gameplay, "Current enemy can't find player game object");
                return;
            }

            Player = playerGo.transform;
        }

        protected abstract void Move();

        protected abstract void Attack();
    }
}