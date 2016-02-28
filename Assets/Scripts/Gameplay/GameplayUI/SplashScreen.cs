using DevilMind;
using UnityEngine;

namespace Ritualist.UI
{
    public class SplashScreen : MonoBehaviour
    {
        public void OnLoad()
        {
            SceneLoader.Instance.LoadStage(1);
        } 
    }
}