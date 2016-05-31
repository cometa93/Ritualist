using System.Collections.Generic;
using DevilMind;
using UnityEngine;

namespace Fading.InteractiveObjects
{
    public class ActivatedGate : DevilBehaviour
    {
        private class Config
        {
            private bool Unlocked;
            private bool Active;
        }

        [SerializeField] List<OrderLockPuzzle> _locks;

        private void OnLoadedObjectState(object stateLoaded)
        {
            var config = stateLoaded as Config;
            if (config == null)
            {
                return;
            }
        }

        protected override void Awake()
        {
            LoadState(OnLoadedObjectState);
            base.Awake();
        }


    }
}