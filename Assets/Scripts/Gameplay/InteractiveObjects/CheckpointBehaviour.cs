using DevilMind;
using UnityEngine;

namespace Fading
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CheckpointBehaviour : MonoBehaviour
    {
        private const string AnimatorIsActiveParameterName = "IsChecked";
        private const string AnimatorActivateTriggerName = "Checked";

        [SerializeField] private bool _savePoint;
        [SerializeField] private int _checkpointNumber;
        [SerializeField] private ParticleSystem _checkpointActive;
        [SerializeField] private Animator _animator;

        private bool _isActive;
        private bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                _animator.SetBool(AnimatorIsActiveParameterName,_isActive);
            }
        }

        private Collider2D _collider2D;
        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
            _collider2D.isTrigger = true;
        }

        private void Start()
        {
            Setup();
        }

        private void Animate()
        {
            _animator.SetTrigger(AnimatorActivateTriggerName);
        }

        private void Setup()
        {
            var save = GameMaster.GameSave.CurrentSave;
            if (save == null)
            {
                Log.Warning(MessageGroup.Gameplay, "Save is null");
                IsActive = true;
                return;
            }

            if (save.StageNumber < SceneLoader.Instance.CurrentStage)
            {
                IsActive = false;
                return;
            }

            if (save.Checkpoint < _checkpointNumber)
            {
                IsActive = false;
            }

            IsActive = true;
        }

        private void OnAnimationEnd()
        {
            IsActive = true;
        }

        private void OnTriggerEnter2D(Collider2D collider2D)
        {

            if (collider2D.tag == "Player")
            {
                if (IsActive)
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

            save.StageNumber = SceneLoader.Instance.CurrentStage;
            save.Checkpoint = _checkpointNumber;
            if (_savePoint)
            {
                GameMaster.GameSave.SaveCurrentGameProgress();
            }
        }
    }
}