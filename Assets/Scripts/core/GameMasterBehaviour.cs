using System.Runtime.InteropServices;
using Ritualist.Controller;
using UnityEngine;

namespace DevilMind
{
    public class GameMasterBehaviour : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void Update()
        {
            GameMaster.Instance.MainLoop(Time.deltaTime);
            MyInputManager.Update();
        }

    }
}