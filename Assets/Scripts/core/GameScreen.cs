using UnityEngine;

namespace DevilMind
{
    public class GameScreen : DevilBehaviour
    {
        public bool HasButtonBack;
        public bool HasVisibleTopBar;
        public System.Action LoadedAction;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (LoadedAction != null)
            {
                LoadedAction();
            }
        }
    }
}