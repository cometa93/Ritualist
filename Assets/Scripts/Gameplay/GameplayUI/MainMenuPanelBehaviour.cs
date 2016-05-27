using DevilMind;
using UnityEngine;

namespace Fading.UI
{
    public class MainMenuPanelBehaviour : DevilBehaviour
    {
        [SerializeField] private GameObject _firstSelected;

        protected override void Start()
        {
            if (MainCanvasBehaviour.EventSystem != null)
            {
                MainCanvasBehaviour.EventSystem.SetSelectedGameObject(_firstSelected);
            }
            base.Start();
        }

        public void OnClickStartGame()
        {
            GameMaster.GameSave.LoadCurrentGame();
        }

        public void OnClickExitGameButton()
        {
            Application.Quit();
        }

        public void OnClickSettingsButton()
        {
            Log.Warning(MessageGroup.Gui, "Settings are not implemented yet.");
        }

        public void OnClickCreditsButton()
        {
            Log.Warning(MessageGroup.Gui, "Credits are not implemented yet");
        }
    }
}