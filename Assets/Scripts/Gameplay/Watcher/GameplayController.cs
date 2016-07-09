using System;
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
        
        private Transform _currentActiveCheckpoint;

        private readonly Dictionary<SkillTargetType, List<SkillTarget>> _targets = new Dictionary<SkillTargetType, List<SkillTarget>>();

        public static void SetupCharacter()
        {
            Instance.SetupCharacterObject();
            SceneLoader.Instance.StageLoaded();
        }

        public static void CreateGameplayController()
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

        public static void DestroyGameplayController()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
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

        private void SetupCharacterObject()
        {
            if (CharacterTransform != null)
            {
                return;
            }

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

            if (_currentActiveCheckpoint == null)
            {
                Log.Error(MessageGroup.Gameplay, "Checkpoint was not registered");
                SceneLoader.Instance.LoadScene(GameSceneType.MainMenu);
                return;
            }

            var pose = _currentActiveCheckpoint.position;
            pose.z = 0;
            characterObj.position = pose;
        }

        protected override void Awake()
        {
            DontDestroyOnLoad(this);
            if (_instance == null)
            {
                _instance = GetComponent<GameplayController>();
            }

            base.Awake();
        }

        protected override void Start()
        {
            CameraFollower.ResetCameraPosition();
            base.Start();
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

        public void SetCurrentCheckpoint(Transform checkpoint)
        {
            _currentActiveCheckpoint = checkpoint;
        }

    }
}
