using System.Collections.Generic;
using Assets.Scripts.Gameplay.InteractiveObjects;
using DevilMind;
using DevilMind.Utils;
using UnityEngine;

namespace Ritualist.AI
{
    public abstract class AbstractEnemy : SkillTarget
    {
        private Transform _player;
        protected Transform Player
        {
            get
            {
                if (_player == null)
                {
                    var playerGo = GameObject.FindGameObjectWithTag("Player");
                    if (playerGo == null)
                    {
                        Log.Error(MessageGroup.Gameplay, "Current enemy can't find player game object");
                        return null;
                    }

                    _player = playerGo.transform;
                }

                return _player;
            }   
        }

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
        }

        protected abstract void Move();

        protected abstract void Attack();
    }
}