using DevilMind;
using UnityEngine;

namespace Fading.UI
{
    public class SplashScreen : MonoBehaviour
    {
        public void OnLoad()
        {
            SceneLoader.Instance.LoadScene(GameSceneType.MainMenu);
        } 
    }
}