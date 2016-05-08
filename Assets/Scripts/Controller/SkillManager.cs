using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.InteractiveObjects;
using DevilMind;
using Fading.Settings;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;
using Random = UnityEngine.Random;

namespace Fading.Controller
{
    public class SkillManager : DevilBehaviour
    {
        [SerializeField] private Transform _catchPointPrefab;
        [SerializeField] private GameObject _protectionSkillObject;
        [SerializeField] private Transform _characterTransform;

        private bool _protectionFieldActivated;
        private bool _characterControllerLocked;
        private int _currentSkillIndex;

        private int CurrentSkillIndex
        {
            get
            {
               return _currentSkillIndex;
            }
            set
            {
                if (GameSettings.SkillOrder.ContainsKey(value) == false)
                {
                    Log.Info(MessageGroup.Gameplay,
                        "Skill order in setting doesn't contain _current skill index : " + _currentSkillIndex);
                    return;
                }
                _currentSkillIndex = value;
                GameMaster.Hero.ChangeSkill(GameSettings.SkillOrder[_currentSkillIndex]);
            }
        }

        protected override void Awake()
        {
            EventsToListen.Add(EventType.RightTriggerReleased);
            EventsToListen.Add(EventType.RightTriggerClicked);
            EventsToListen.Add(EventType.ButtonReleased);
            EventsToListen.Add(EventType.CharacterDied);
            EventsToListen.Add(EventType.CharacterChanged);
            Setup();
            base.Awake();
        }

        private void Setup()
        {
            _protectionSkillObject.SetActive(false);
            _protectionSkillObject.transform.localScale = Vector3.zero;
            _characterControllerLocked = false;
        }

        protected override void OnEvent(Event gameEvent)
        {
            switch (gameEvent.Type)
            {
                case EventType.RightTriggerClicked:
                    ActivateSkill();
                    break;

                case EventType.RightTriggerReleased:
                    DeactivateSkill();
                    break;

                case EventType.CharacterChanged:
                    _characterControllerLocked = !_characterControllerLocked;
                    break;

                case EventType.CharacterDied:
                    _characterControllerLocked = true;
                    break;

                case EventType.ButtonReleased:
                    ChangeSkill((InputButton)gameEvent.Parameter);
                    break;

            }
            base.OnEvent(gameEvent);
        }

        public void DeactivateSkill()
        {
            if (_protectionFieldActivated)
            {
                _protectionFieldActivated = false;
                DeactivateProtectionField();
            }
        }

        public void ActivateSkill()
        {
            if (_characterControllerLocked)
            {
                return;
            }
            var currentSkill = GameMaster.Hero.CurrentHeroSkill;
            switch (currentSkill.Effect)
            {
                case SkillEffect.Catch:
                    ShootCatchPoint();
                    break;
                case SkillEffect.ProtectionField:
                    ActivateProtectionField();
                    _protectionFieldActivated = true;
                    break;
            }
        }

        #region Catch Skill

        private CatchPoint PrepareCatchPoint()
        {
            Transform spawnedPrefab =
                Instantiate(_catchPointPrefab, transform.position, Quaternion.identity) as Transform;
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
            GameMaster.Hero.Stats.Power -= currentSkill.PowerCost;
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
            var targets = new List<SkillTarget>();
            var enemyTargets = GameplayController.Instance.GetTargets(SkillTargetType.Enemy);
            if (enemyTargets != null && enemyTargets.Count > 0)
            {
                targets = enemyTargets;
            }
            else
            {
                targets = GameplayController.Instance.GetTargets(SkillTargetType.CatchPoint);
            }
            
            for (int i = 0, c = targets.Count; i < c; ++i)
            {
                var target = targets[i];
                if (range >= target.Distance(_characterTransform.position))
                {
                    list.Add(target);
                }
            }

            list.Sort((target1, target2) =>
            {
                return target1.Distance(_characterTransform.position) >= target2.Distance(_characterTransform.position) ? 1 : -1;
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

        #region Protection Field

        private void ActivateProtectionField()
        {
            iTween.Stop(_protectionSkillObject);
            _protectionSkillObject.SetActive(true);
            _protectionSkillObject.ScaleTo(Vector3.one, 0.2f, 0);
        }

        private void DeactivateProtectionField()
        {
            iTween.Stop(_protectionSkillObject);
            iTween.ScaleTo(_protectionSkillObject,new Hashtable
            {
                {iT.ScaleTo.scale, Vector3.zero },
                {iT.ScaleTo.time, 0.2f },
                {iT.ScaleTo.oncomplete, "OnFieldDeactivated" }
            });
        }

        private void OnFieldDeactivated()
        {
            _protectionSkillObject.SetActive(false);
        }

        #endregion

        #region Manager

        private void ChangeSkill(InputButton type)
        {
            switch (type)
            {
                 case InputButton.SkillButton1:
                    CurrentSkillIndex = 1;
                    break;
                 case InputButton.SkillButton2:
                    CurrentSkillIndex = 2;
                    break;
                 case InputButton.SkillButton3:
                    CurrentSkillIndex = 3;
                    break;
                 case InputButton.SkillButton4:
                    CurrentSkillIndex = 4;
                    break;
            }
        }

        #endregion
    }
}
