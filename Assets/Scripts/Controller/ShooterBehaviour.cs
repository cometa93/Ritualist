using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.InteractiveObjects;
using DevilMind;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;
using Random = UnityEngine.Random;

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
            var currentSkill = GameMaster.Hero.CurrentSkillEffect;
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
            catchPoint.transform.parent = _worldParent;
            var targets = GetPossibleTargets(GameMaster.Hero.CurrentHeroSkill);
            catchPoint.Shoot(targets.Count > 0 ? targets[0].Position : GetMissShoot(GameMaster.Hero.CurrentHeroSkill));
        }


        private List<SkillTarget> GetPossibleTargets(HeroSkill currentSkill)
        {
            if (currentSkill == null)
            {
                return null;
            }

            switch (currentSkill.Effect)
            {
                case SkillEffect.Catch:
                    return GetCatchPointsTargets(currentSkill.Range);
                default:
                    return null;
            }
            //TODO IMPLEMENT MORE !
        }

        private List<SkillTarget> GetCatchPointsTargets(float range)
        {
            var list = new List<SkillTarget>();
            var possibleTargets = GameplayController.Instance.GetTargets(SkillTargetType.CatchPoint);
            for (int i = 0, c = possibleTargets.Count; i < c; ++i)
            {
                var target = possibleTargets[i];
                if (range <= target.Distance(_spawnPoint.position))
                {
                    list.Add(target);
                }
            }
            list.Sort((target1, target2) =>
            {
                return target1.Distance(_spawnPoint.position) >= target2.Distance(_spawnPoint.position) ? 1 : -1;
            });
            return list;
        }

        private Vector2 GetMissShoot(HeroSkill currentSkill)
        {
            return new Vector2(Random.Range(0, 1f), Random.Range(0, 1f))*currentSkill.Range;
        }
        #endregion
    }
}
