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
        
        private int _currentPointIndex;
        private long _stopFrame = 0;

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
                return;
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
            Transform spawnedPrefab = Instantiate(_arrowPrefab, transform.position, Quaternion.identity) as Transform;
            if (spawnedPrefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "Spawned prefab is null");
                return null;
            }

            spawnedPrefab.transform.parent = transform;
            spawnedPrefab.localScale = Vector3.one;
            spawnedPrefab.localPosition = Vector3.zero;
            spawnedPrefab.localEulerAngles = Vector3.zero;
            return spawnedPrefab.GetComponent<BurningSoulMissle>();
        }

        private void ShootToPlayer()
        {
            BurningSoulMissle catchPoint = SpawnMissle();
            if (catchPoint == null)
            {
                Debug.LogWarning("prepared rune is null");
                return;
            }

            catchPoint.Shoot(Player.position);
        }


    }
}