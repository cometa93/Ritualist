using DevilMind;
using UnityEngine;
using UnityEngine.UI;

namespace Fading.UI
{
    public class PauseMenuBehaviour : DevilBehaviour
    {
        [SerializeField] private Button _firstSelectedButton;

        protected override void OnEnable()
        {
            if (MainCanvasBehaviour.EventSystem != null)
            {
                MainCanvasBehaviour.EventSystem.SetSelectedGameObject(_firstSelectedButton.gameObject);
                _firstSelectedButton.Select();
            }

            base.OnEnable();
        }

        public void OnClickLoadLastCheckpoint()
        {
            UnpauseGameplay();
            GameMaster.GameSave.LoadCurrentGame();
        }

        public void OnClickReturnToMainMenuButton()
        {
            UnpauseGameplay();
            SceneLoader.Instance.LoadScene(GameSceneType.MainMenu);
            MainCanvasBehaviour.DisablePanel(UIType.GameplayGUI);
        }

        public void OnClickResumePauseMenu()
        {
            UnpauseGameplay();
        }

        private void UnpauseGameplay()
        {
            GameplayController.IsGameplayPaused = false;
        }


    }
}