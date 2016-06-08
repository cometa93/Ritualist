using DevilMind;
using UnityEngine;
using UnityEngine.UI;

namespace Fading.UI
{
    public class MainMenuPanelBehaviour : DevilBehaviour
    {
        [SerializeField] private GameObject _firstSelected;
        private Button _firstSelectedButton;

        protected override void OnEnable()
        {
            _firstSelectedButton = _firstSelected.GetComponent<Button>();
            base.OnEnable();
        }

        protected override void Start()
        {
            if (MainCanvasBehaviour.EventSystem != null)
            {
                MainCanvasBehaviour.EventSystem.SetSelectedGameObject(_firstSelected);

                if (_firstSelectedButton != null)
                {
                    _firstSelectedButton.Select();
                }
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