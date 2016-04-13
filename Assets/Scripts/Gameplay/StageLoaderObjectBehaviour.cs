using DevilMind;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class StageLoaderObjectBehaviour : DevilBehaviour
    {
        [SerializeField] private int _stageNumberToLoad;
        [SerializeField] private LayerMask _characterLayerMask;

        private bool _isLoaded;

        protected override void OnEnable()
        {
            _isLoaded = false;
        }

        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (_isLoaded == false && collider2D.tag == "Player") 
            {
                _isLoaded = true;
                SceneLoader.Instance.LoadStage(_stageNumberToLoad);
            }
        }
    }
}