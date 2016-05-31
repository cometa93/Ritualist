using System.Collections.Generic;
using DevilMind;
using UnityEngine;

namespace Fading.InteractiveObjects
{
    public class ActivatedGate : DevilBehaviour
    {
        private class SaveState
        {
            private bool Unlocked;
            private bool Active;
        }

        [SerializeField] List<OrderLockPuzzle> _locks;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}