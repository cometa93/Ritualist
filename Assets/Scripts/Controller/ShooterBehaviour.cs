using System;
using UnityEngine;
using System.Collections;
using DevilMind;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.Controller
{
    public class ShooterBehaviour : DevilBehaviour
    {
        
        [SerializeField] private Transform _catchFieldGeneratorPrefab;
        [SerializeField] private Transform _runePrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _worldParent;
        
        protected override void Awake()
        {
            EventsToListen.Add(EventType.RightTriggerReleased);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.RightTriggerReleased)
            {
                Shoot();
            }
            base.OnEvent(gameEvent);
        }

        public void Shoot()
        {
            var currentSkill = GameMaster.Hero.CurrentSkill;
            switch (currentSkill)
            {
                case SkillEffect.Catch:
                    ShootCatchPoint();
                    break;
                case SkillEffect.Stunt:
                    break;
                case SkillEffect.Damage:
                    break;
                case SkillEffect.Freeze:
                    break;
            }
        }

        #region Catch Skill

        private CatchPoint PrepareCatchPoint()
        {
            Transform spawnedPrefab = Instantiate(_runePrefab, _spawnPoint.position, Quaternion.identity) as Transform;
            if (spawnedPrefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "Spawned prefab is null");
                return null;
            }
            
            spawnedPrefab.transform.parent = transform;
            spawnedPrefab.localScale = Vector3.one;
            spawnedPrefab.localPosition = Vector3.zero;
            spawnedPrefab.localEulerAngles = Vector3.zero;
            return spawnedPrefab.GetComponent<CatchPoint>();
        }

        private void ShootCatchPoint()
        {
            CatchPoint catchPoint = PrepareCatchPoint();
            if (catchPoint == null)
            {
                Debug.LogWarning("prepared rune is null");
                return;
            }
            catchPoint.Shoot();
            catchPoint.transform.parent = _worldParent;
        }

        #endregion
    }
}
