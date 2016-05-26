using UnityEngine;

namespace Fading
{
    public class ConsoleText : MonoBehaviour
    {
        private static ConsoleText _instanceText; 
        [SerializeField] UnityEngine.UI.Text _consoleText;
 
        public static ConsoleText Instance
        {
            get { return _instanceText; }
        }

        void Awake()
        {
            _instanceText = this;
        }

        public void UpdateText(string textToShow)
        {
            _consoleText.text = textToShow;
        }

    }
}