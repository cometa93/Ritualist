using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.InteractiveObjects;
using DevilMind;
using DevilMind.Utils;
using EventType = DevilMind.EventType;

namespace Ritualist
{
    public class GameplayController : DevilBehaviour
    {
        private GameObject _magicFieldPrefab;
        private GameObject _gameplayGui;
        private MagicField.Config _config;
        private readonly List<Transform> _checkPoints = new List<Transform>();
        private readonly Dictionary<SkillTargetType, List<SkillTarget>> _targets = new Dictionary<SkillTargetType, List<SkillTarget>>(); 
        
        private static GameplayController _instance;
        public static GameplayController Instance
        {
            get
            {
                return _instance;
            }
        }

        protected override void Awake()
        {
            _instance = GetComponent<GameplayController>();
            SetupCheckPoints();
            SetupMagicFieldPrefab();
            SetupGameplayGui();
            SetupCharacterObject();
            SetupTargets();
            SceneLoader.Instance.StageLoaded();
            base.Awake();
        }

        private void SetupTargets()
        {
            var gos = GameObject.FindGameObjectsWithTag("Target");
            for (int i = 0, c = gos.Length; i < c; ++i)
            {
                GameObject go = gos[i];
                if (go == null)
                {
                    continue;
                }

                SkillTarget target = go.GetComponent<SkillTarget>();
                if (target == null)
                {
                    Log.Warning(MessageGroup.Gameplay,
                        "target with name : " + go.name + " is not a target or dont have script attached");
                    continue;
                }
                RegisterTarget(target);
            }
        }

        //TODO Make spawna points checkpoints and save character state;
        private void SetupCheckPoints()
        {
            var go = GameObject.FindGameObjectWithTag("SpawnPoint");
            if (go == null)
            {
                Log.Error(MessageGroup.Gameplay, "Couldn't find checkpoints parent");
                return;
            }

            foreach (Transform t in go.transform)
            {
                _checkPoints.Add(t);
            }

            _checkPoints.Sort((transform1, transform2) =>
            {
                int t1, t2;

                if (int.TryParse(transform1.name, out t1) == false)
                {
                    return -1;
                };

                if (int.TryParse(transform2.name, out t2) == false)
                {
                    return 1;
                };

                return t1 >= t2 ? 1:-1;
            });
        }

        private void SetupCharacterObject()
        {
            var prefab = ResourceLoader.LoadCharacter();
            if (prefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "Character transform is null");
                return;
            }

            var go = Instantiate(prefab);
            if (go == null)
            {
                Log.Error(MessageGroup.Gameplay, "Couldn't spawn character gameobject");
                return;
            }
            //Seting up character position on spawn point;
            var characterObj = go.transform.FindChild("MainCharacter");
            if (characterObj == null)
            {
                Log.Error(MessageGroup.Gameplay, "Can't find character in character transform");
                return;
            }

            if (_checkPoints.Count <= 0)
            {
                Log.Error(MessageGroup.Gameplay, "Checkpoints are empty !");
                return;
            }

            characterObj.transform.position = _checkPoints[0].position;
        }

        private void SetupMagicFieldPrefab()
        {
            _magicFieldPrefab = ResourceLoader.LoadMagicField();
            if (_magicFieldPrefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "MagicField prefab is null");
            }
        }

        private void SetupGameplayGui()
        {
            var prefab = ResourceLoader.LoadGameplayGUI();
            if (prefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "Can't get gameplay gui object");
                return;
            }

            if (_gameplayGui != null)
            {
                return;
            }

            _gameplayGui = Instantiate(prefab) as GameObject;
            if (_gameplayGui == null)
            {
                Log.Error(MessageGroup.Gameplay, "Couldn't instantiate gui");
            }
        }

        public void RegisterTarget(SkillTarget target)
        {
            if (_targets.ContainsKey(target.Type) == false)
            {
                _targets[target.Type] = new List<SkillTarget>();
            }

            if (_targets[target.Type].Contains(target) == false)
            {
                _targets[target.Type].Add(target);
            }
        }

        public void UnregisterTarget(SkillTarget target)
        {
            if (_targets.ContainsKey(target.Type) == false)
            {
                return;
            }

            if (_targets[target.Type].Contains(target))
            {
                _targets[target.Type].Remove(target);
            }
        }

        public List<SkillTarget> GetTargets(SkillTargetType type)
        {
            if (_targets.ContainsKey(type) == false)
            {
                return new List<SkillTarget>();
            }

            return _targets[type];
        } 

        #region Catch Skill Helper
        public void PlacePoint(CatchPoint point)
        {
            var config = _config ?? ( _config = new MagicField.Config()
            {
                LongLife = GameMaster.Hero.Skills[SkillEffect.Catch].LongLife,
                CatchPoints = new List<CatchPoint>()
            });

            config.CatchPoints.Add(point);

            if (config.IsFull)
            {
                _config = null;
                var magicField = Instantiate(_magicFieldPrefab);
                var goPosition = magicField.transform.position;
                goPosition.z = 15;
                magicField.transform.position = goPosition;
                StartCoroutine(TimeHelper.RunAfterFrames(5, () =>
                {
                    magicField.GetComponent<MagicField>().Setup(config);
                }));
            }
        }
        #endregion
    }
}
