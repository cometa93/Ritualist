using System.Collections;
using DevilMind;
using Ritualist.Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Ritualist.UI
{
    public class SplashScreenControll : MonoBehaviour
    {
        [SerializeField] private Transform _pressAInfo;
        [SerializeField] private Transform _infoScreen;
        private bool _available;
        private int counter = 0;

        private void Awake()
        {
            counter = 0;
            _pressAInfo.gameObject.SetActive(false);
            _available = false;
            StartCoroutine(WaitAWhile());
        }

        private void Update()
        {

            if (MyInputManager.IsButtonDown(InputButton.A))
            {
                counter ++;
                _available = false;
                if (counter == 1)
                {
                    _infoScreen.gameObject.SetActive(true);
                }
                if (counter > 1)
                {
                    SceneManager.LoadScene("Gameplay");
                    return;
                }
                StartCoroutine(WaitAWhile());                
            }            
        }

        private IEnumerator WaitAWhile()
        {
            yield return new WaitForSeconds(3f);
            if (counter == 0)
            {
                _available = true;
                _pressAInfo.gameObject.SetActive(true);
            }
            else if (counter == 1)
            {
                _available = true;
            }

        }
    }
}