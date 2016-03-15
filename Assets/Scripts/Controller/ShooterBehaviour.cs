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
        [SerializeField] private Transform _catchPointPrefab;
        
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
            }
        }

        #region Catch Skill

        private CatchPoint PrepareCatchPoint()
        {
            Transform spawnedPrefab = Instantiate(_catchPointPrefab, transform.position, Quaternion.identity) as Transform;
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

            catchPoint.transform.parent = transform.root;
            var currentSkill = GameMaster.Hero.CurrentHeroSkill;
            var targets = GetPossibleTargets(currentSkill);
            catchPoint.Shoot(targets.Count > 0 ? targets[0].Position : GetMissShoot(GameMaster.Hero.CurrentHeroSkill));
            GameMaster.Hero.Stats.Power  -= currentSkill.PowerCost;
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
        }

        private List<SkillTarget> GetCatchPointsTargets(float range)
        {
            var list = new List<SkillTarget>();
            var possibleTargets = GameplayController.Instance.GetTargets(SkillTargetType.CatchPoint);
            for (int i = 0, c = possibleTargets.Count; i < c; ++i)
            {
                var target = possibleTargets[i];
                if (range >= target.Distance(transform.position))
                {
                    list.Add(target);
                }
            }
            list.Sort((target1, target2) =>
            {
                return target1.Distance(transform.position) >= target2.Distance(transform.position) ? 1 : -1;
            });
            return list;
        }

        private Vector2 GetMissShoot(HeroSkill currentSkill)
        {
            var random = new Vector2(Random.Range(0, 1f), Random.Range(0, 1f))*currentSkill.Range;
            random.x += transform.position.x;
            random.y += transform.position.y;
            return random;
        }
        #endregion
    }
}
