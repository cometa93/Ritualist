using DevilMind;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class StageLoaderObjectBehaviour : DevilBehaviour
    {
        [SerializeField] private int _stageNumberToLoad;


        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            SceneLoader.Instance.LoadStage(_stageNumberToLoad);
        }
    }
}