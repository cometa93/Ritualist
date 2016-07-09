using System.Collections.Generic;
using DevilMind;
using UnityEngine;

namespace Fading.InteractiveObjects
{
    public class ActivatedGate : DevilBehaviour
    {
        [SerializeField] List<OrderLockPuzzle> _locks;

        private void OnLoadedObjectState(object stateLoaded)
        {
            var config = stateLoaded as ActivatedGateConfig;
            if (config == null)
            {
                return;
            }
        }

        protected override void Awake()
        {
            LoadState(GameMaster.GameSave.CurrentSave.ActivatedDoorsConfig ,OnLoadedObjectState);
            base.Awake();
        }


    }
}