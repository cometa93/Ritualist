using System.Collections;
using DevilMind;
using UnityEngine;

namespace Ritualist.AI.Enemies
{
    public class BurningSoul : AbstractEnemy
    {
        [Header("Move Animation Helpers")]
        [Range(0,100)][SerializeField] private int FrameOffsetBetweenStartAndStop;
        [SerializeField] private float TimeForOneUnit;
        [SerializeField] private System.Collections.Generic.List<Transform> _pointsToMoveWithin;

        [Header("Enemy Setup")]
        [SerializeField] private float _attackRange;
        [SerializeField] private GameObject _arrowPrefab;
        [Range(0, 5f)] [SerializeField] private float _attackCooldown;
        [SerializeField] private bool _pingPongMoving;

        private float _currentAttackTime;
        private int _currentPointIndex;
        private long _stopFrame = 0;
        private bool _reversing;

        protected override void Setup()
        {
        }

        protected override void Move()
        {
            if (IsTargetInRange())
            {
                _stopFrame = Time.frameCount;
                StopMoving();
                return;
            }

            if (Time.frameCount - _stopFrame < FrameOffsetBetweenStartAndStop)
            {
                return;
            }

            if (iTween.Count(gameObject) > 0)
            {
                return;
            }

            StartMoving();
        }

        protected override void Attack()
        {
            if (IsTargetInRange() == false)
            {
                _currentAttackTime = 0;
                return;
            }

            if (_currentAttackTime <= 0.01f)
            {
                ShootToPlayer();
            }
            _currentAttackTime += Time.deltaTime;
            if (_currentAttackTime > _attackCooldown)
            {
                _currentAttackTime = 0;
            }
        }

        private bool IsTargetInRange()
        {
            return Vector2.Distance(Player.position, Position) <= _attackRange;
        }

        private void StopMoving()
        {
            iTween.Stop(gameObject);
        }

        private void StartMoving()
        {
            var distanceToGo = Vector2.Distance(Position, _pointsToMoveWithin[_currentPointIndex].position);
            var time = TimeForOneUnit * distanceToGo;
            var hash = new Hashtable
            {
                {iT.MoveTo.position, _pointsToMoveWithin[_currentPointIndex].position},
                {iT.MoveTo.time, time},
                {iT.MoveTo.oncomplete, "OnMovePointReached"}
            };

            iTween.MoveTo(gameObject,hash);
        }

        private void OnMovePointReached()
        {

            if (_pingPongMoving)
            {
                if (_reversing)
                {
                    if (_currentPointIndex <= 0)
                    {
                        _reversing = false;
                        _currentPointIndex++;
                    }
                    else
                    {
                        _currentPointIndex--;
                    }
                }
                else
                {
                    if (_currentPointIndex == _pointsToMoveWithin.Count - 1)
                    {
                        _currentPointIndex--;
                        _reversing = true;
                    }
                    else
                    {
                        _currentPointIndex++;
                    }
                }

                _stopFrame = Time.frameCount;
                return;
            }

            if (_currentPointIndex == _pointsToMoveWithin.Count - 1)
            {
                _currentPointIndex = 0;
            }
            else
            {
                _currentPointIndex++;
            }

            _stopFrame = Time.frameCount;
        }

        private BurningSoulMissle SpawnMissle()
        {
            GameObject spawnedPrefab = Instantiate(_arrowPrefab, transform.position, Quaternion.identity) as GameObject;
            if (spawnedPrefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "Spawned prefab is null");
                return null;
            }

            spawnedPrefab.gameObject.SetActive(true);
            spawnedPrefab.transform.parent = transform.root;
            spawnedPrefab.transform.localScale = Vector3.one;
            spawnedPrefab.transform.position = transform.position;
            spawnedPrefab.transform.localEulerAngles = Vector3.zero;
            return spawnedPrefab.GetComponent<BurningSoulMissle>();
        }

        private void ShootToPlayer()
        {
            BurningSoulMissle burningSoulMissle = SpawnMissle();
            if (burningSoulMissle == null)
            {
                Debug.LogWarning("prepared rune is null");
                return;
            }

            burningSoulMissle.Shoot(Player.position);
        }


    }
}