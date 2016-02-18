using System.Collections;
using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.AI
{
    public class BasicEnemyBehaviour : DevilBehaviour
    {
        public EnemyType EnemyType;
        Transform _myTransform;
        private Transform _characterToFollow;
        [SerializeField] protected float AttackCooldown;
        public int Damage;

        [SerializeField] private int _unitsPerSecond;
        [SerializeField] private List<Transform> _checkersForTrap;
        [SerializeField] private LayerMask _magicFieldMask;
        [SerializeField] private ParticleSystem _destroyParticles;

        private bool _catched;
        private bool _trapFounded;
        private bool _canAttack;


        protected override void Awake()
        {
            _characterToFollow = GameObject.FindGameObjectWithTag("Player").transform;
            GameplayController.Instance.EnemyCounter++;
            _myTransform = transform;
            _canAttack = true;
                        
            base.Awake();
        }

        private void CheckDistance()
        {

            if (Vector3.Distance(_myTransform.position, _characterToFollow.position) <= 2.5 && _canAttack)
            {
                GameplayController.Instance.PerformAttackOnHero(Damage);
                StartCoroutine(WaitUntillCanAttackAgain());
                _canAttack = false;
            }
        }

        protected override void Update()
        {
            if (GameplayController.Instance.GameEnded)
            {
                return;
            }

            if (_characterToFollow == false)
            {
                return;
            }
            
            var targetVector = Vector3.MoveTowards(_myTransform.position, _characterToFollow.position, _unitsPerSecond * Time.deltaTime);
            _myTransform.position = targetVector;
        
            var vector = _myTransform.localScale;
            vector.x = _myTransform.position.x > _characterToFollow.position.x ?  1 :  - 1;
            _myTransform.localScale = vector;
        
            CheckDistance();
        }

        private void FixedUpdate()
        {

            if (GameplayController.Instance.GameEnded)
            {
                return;
            }

            if (_trapFounded)
            {
                return;
            }
            CheckForCatch();
            if (_catched == false)
            {
                LookoutForTrap();
            }
        }

        private void LookoutForTrap()
        {
            for (int i = 0, c = _checkersForTrap.Count; i < c; ++i)
            {
                var checker = _checkersForTrap[i];
                if (Physics2D.OverlapCircle(checker.position, 0.1f, _magicFieldMask))
                {
                    _trapFounded = true;
                    break;
                }
            }
            if (_trapFounded)
            {
                OnTrapFounded();
            }
        }

        private void CheckForCatch()
        {
            if (_catched == false && Physics2D.OverlapCircle(_myTransform.position, 0.2f, _magicFieldMask))
            {
                _catched = true;
                OnEnemyDied();
            }
        }

        private void OnEnemyDied()
        {
            _destroyParticles.gameObject.SetActive(true);
            _destroyParticles.Play(true);
            GameplayController.Instance.EnemyKilledCounter++;
            GameMaster.Events.Rise(EventType.EnemyDied);
            GameplayController.Instance.SpawnEnemy(EnemyType);
            //TEMPORARY SPAWNING INFINITE !
            GameplayController.Instance.SpawnEnemy(EnemyType);
            iTween.ScaleTo(gameObject, iTween.Hash(
                 "scale", Vector3.zero,
                 "time", 0.5f,
                 "oncomplete", "OnDeadAnimEnd",
                 "easetype", iTween.EaseType.easeOutCubic
             ));
        }

        private void OnDeadAnimEnd()
        {
            Destroy(gameObject);
        }

        private void OnTrapFounded()
        {

            GameplayController.Instance.SpawnEnemy(EnemyType);
            iTween.ScaleTo(gameObject, iTween.Hash(
                 "scale", Vector3.zero,
                 "time", 0.5f,
                 "oncomplete", "OnDeadAnimEnd",
                 "easetype", iTween.EaseType.easeOutSine
             ));
        }

        private IEnumerator WaitUntillCanAttackAgain()
        {
            yield return new WaitForSeconds(AttackCooldown);
            _canAttack = true;
        }
    }
}