using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.InteractiveObjects;
using DevilMind;
using Fading.InteractiveObjects;
using Fading.UI;
using SRDebugger.UI.Controls.Data;
using EventType = DevilMind.EventType;

namespace Fading
{
    public class GameplayController : DevilBehaviour
    {
        public static bool GameplayControllerCreated = false;

        private static bool _isLoadingFromSave;
        private static bool _isEnteringFromBack;

        private static bool _isGameplayPaused;
        public static bool IsGameplayPaused
        {
            get { return _isGameplayPaused; }
            set
            {
                if (value == _isGameplayPaused)
                {
                    return;
                }

                _isGameplayPaused = value;
                GameMaster.Events.Rise(EventType.PauseGame,_isGameplayPaused);
            }
        }

        private GameObject _gameplayGui;

        private readonly List<Transform> _checkPoints = new List<Transform>();
        private readonly Dictionary<SkillTargetType, List<SkillTarget>> _targets = new Dictionary<SkillTargetType, List<SkillTarget>>();
        
        public static void CreateGameplayControllerOnStageLoaded()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameplayController");
                _instance = go.AddComponent<GameplayController>();
                if (_instance == null)
                {
                    Log.Warning(MessageGroup.Gameplay, "Gameplay controller is null buuu....");
                }
            }
        }

        private static GameplayController _instance;
        public static GameplayController Instance
        {
            get
            {
                return _instance;
            }
        }
        public GameObject CharacterTransform { get; private set; }

        protected override void Awake()
        {
            _instance = GetComponent<GameplayController>();
            SetupCheckPoints();
            SetupCharacterObject();
            SetupTargets();
            SetupWorldBehaviour();
            SceneLoader.Instance.StageLoaded();
            base.Awake();
        }

        protected override void Start()
        {
            CameraFollower.ResetCameraPosition();
            base.Start();
        }

        private void SetupWorldBehaviour()
        {
            var go = GameObject.Find("World");
            if (go == null)
            {
                Log.Error(MessageGroup.Gameplay, "Scene doesn't contain object with name World !");
                return;
            }

            go.AddComponent<WorldBehaviour>();
            Log.Info(MessageGroup.Gameplay, "World behaviour added to world object");
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

            CharacterTransform = go;
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
