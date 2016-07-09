using System.Collections.Generic;
using DevilMind;
using Fading;
using UnityEngine;

namespace Fading.InteractiveObjects
{
    public class StageLoaderObjectBehaviour : DevilBehaviour
    {
        [SerializeField] private List<int> _stageNumbersToLoad;
        [SerializeField] private LayerMask _characterLayerMask;
        
        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (collider2D.tag != "Player") return;
            SceneLoader.Instance.LoadStages(_stageNumbersToLoad);
        }
    }
}