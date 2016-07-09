using System.Collections.Generic;
using DevilMind;
using Fading.InteractiveObjects;
using UnityEngine;

namespace Fading
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CheckpointBehaviour : DevilBehaviour
    {
        private const string AnimatorIsActiveParameterName = "IsChecked";
        private const string AnimatorActivateTriggerName = "Checked";

        [SerializeField] private bool _savePoint;
        [SerializeField] private bool _gameStartingPoint;
        [SerializeField] private int _stageNumber;
        [SerializeField] private Animator _animator;

        #region SaveAble Object config region

        private CheckpointConfig _config;

        private void OnConfigLoaded(object config)
        {
            if (config == null)
            {
                _config = new CheckpointConfig
                {
                    StageNumber = _stageNumber
                };
                return;
            }

            _config = config as CheckpointConfig;
            if (_config == null)
            {
                Log.Warning(MessageGroup.Common,
                    "Can't cas config which is not null to Checkpoint config possibly other" +
                    "config is saved under the same uniqueID that's big problem");
                _config = new CheckpointConfig
                {
                    StageNumber = _stageNumber
                };
            }
        }

        #endregion

        private bool _isActivatedAlready;
        private bool IsActivatedAlready
        {
            get { return _isActivatedAlready; }
            set
            {
                _isActivatedAlready = value;
                if (_animator != null)
                {
                    _animator.SetBool(AnimatorIsActiveParameterName, _isActivatedAlready);
                }
            }
        }

        private Collider2D _collider2D;

        protected override void Awake()
        {
            LoadState(GameMaster.GameSave.CurrentSave.CheckpointsConfigs, OnConfigLoaded);
            _collider2D = GetComponent<Collider2D>();
            _collider2D.isTrigger = true;
            base.Awake();
        }
        
        protected override void Start()
        {
            var save = GameMaster.GameSave.CurrentSave;

            if (_gameStartingPoint && save == null ||
                ( _gameStartingPoint && save != null && save.CheckpointUniqueId == null))
            {
                GameplayController.Instance.SetCurrentCheckpoint(transform);
            }

            if (save != null &&
                save.CheckpointUniqueId != null &&
                _uniqueID == save.CheckpointUniqueId)
            {
                GameplayController.Instance.SetCurrentCheckpoint(transform);
            }

            Setup();
        }

        private void Animate()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(AnimatorActivateTriggerName);
            }
        }

        private void Setup()
        {
            var save = GameMaster.GameSave.CurrentSave;
            if (save == null)
            {
                Log.Warning(MessageGroup.Gameplay, "Save is null");
                IsActivatedAlready = true;
                return;
            }

            IsActivatedAlready = _config.IsActivated;
        }

        private void OnAnimationEnd()
        {
            IsActivatedAlready = true;
        }

        private void OnTriggerEnter2D(Collider2D collider2D)
        {

            if (collider2D.tag == "Player")
            {
                if (IsActivatedAlready)
                {
                    return;
                }

                Animate();
                SaveGame();
            }
        }

        private void SaveGame()
        {
            var save = GameMaster.GameSave.CurrentSave;
            if (save == null)
            {
                Log.Error(MessageGroup.Gameplay, "Current Save is null...");
                return;
            }
            
            if (_savePoint)
            {
                _config.IsActivated = true;
                _config.StageNumber = _stageNumber;
                save.CheckpointUniqueId = _uniqueID;
                SaveState(save.CheckpointsConfigs, _config, true);
            }
        }
    }
}