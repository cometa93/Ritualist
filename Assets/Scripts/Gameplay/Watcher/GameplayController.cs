using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.InteractiveObjects;
using DevilMind;
using Fading.UI;

namespace Fading
{
    public class GameplayController : DevilBehaviour
    {
        private static bool _isLoadingFromSave = false;
        private static bool _isEnteringFromBack = false;

        private GameObject _magicFieldPrefab;
        private GameObject _gameplayGui;
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
            SetupGameplayGui();
            SetupGameplayMenu();
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

            int indexOfCheckpoint = 0;
            if (_isEnteringFromBack)
            {
                indexOfCheckpoint = _checkPoints.Count - 1;
            }

            if (_isLoadingFromSave)
            {
                _isLoadingFromSave = false;
                var save = GameMaster.GameSave.CurrentSave;
                if (save != null)
                {
                    if (save.Checkpoint == 0)
                    {
                        Log.Warning(MessageGroup.Common, "save checkpoint is equal 0. Checkpoints are starting from 1");
                        return;
                    }
                    if (_checkPoints.Count > save.Checkpoint - 1)
                    {
                        indexOfCheckpoint = save.Checkpoint - 1;
                    }
                }
            }


            characterObj.transform.position = _checkPoints[indexOfCheckpoint].position;
            if (_isEnteringFromBack)
            {
                Vector3 theScale = characterObj.transform.localScale;
                theScale.z *= -1;
                characterObj.transform.localScale = theScale;
                Vector3 theRotation = characterObj.transform.localEulerAngles;
                theRotation.y = theScale.z > 0 ? 0 : 180;
                characterObj.transform.localEulerAngles = theRotation;
                _isEnteringFromBack = false;
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

        private void SetupGameplayMenu()
        {
            if (GameplayMenuBehaviour.GameplayMenuCreated)
            {
                return;
            }

            var prefab = ResourceLoader.LoadGameplayMenu();
            if (prefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "Can't get gameplaymenu object");
                return;
            }
            
            Instantiate(prefab);
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

        public static void SetupFromGameSave()
        {
            _isLoadingFromSave = true;
        }

        public static void SetupFromBack()
        {
            _isEnteringFromBack = true;
        }
    }
}
