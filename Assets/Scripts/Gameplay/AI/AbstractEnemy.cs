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

        protected bool IsAlive
        {
            get { return Health > 0; }
        }
        
        protected override void Awake()
        {
            Setup();
            base.Awake();
        }

        protected override void Update()
        {
            if (IsAlive == false)
            {
                return;
            }
            Move();
            Attack();
        }

        protected virtual void FixedUpdate()
        {
            if (IsAlive == false)
            {
                return;
            }
        }

        protected virtual void Setup()
        { 
        }

        protected abstract void Move();

        protected abstract void Attack();

        protected abstract void OnProtectionFieldHitTaken();

        protected abstract void OnProtectionFieldDestroyed();

        protected abstract void OnHitTaken();

        protected abstract void OnDied();

        public void Hit(int damage)
        {
            if (ProteciveField > 0)
            {
                ProteciveField -= damage;
                if (ProteciveField < 0)
                {
                    OnProtectionFieldDestroyed();
                    return;
                }
                OnProtectionFieldHitTaken();
                return;
            }

            Health -= damage;
            if (Health < 0)
            {
                OnDied();
                return;
            }
            OnHitTaken();
        }
    }
}