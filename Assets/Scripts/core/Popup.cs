using UnityEngine;
using UnityEngine.UI;

namespace DevilMind
{
    public class Popup : DevilBehaviour
    {
        #region Properties
        
        public PopupType PopupType;
        public bool HasButtonExit = true;
        
        #endregion

        #region Editor Variables

        [SerializeField] private Transform _exitButtonParent;

        #endregion

        #region Overrided Functions

        protected override void Awake()
        {
            base.Awake();
            SetupPopupButtons();
        }

        #endregion

      
        private void SetupPopupButtons()
        {
        
        }

        private Button SpawnButton(ButtonType type, Transform parent)
        {
            if (parent == null)
            {
                Log.Warning(MessageGroup.Gui, "Trying to spawn button but parent is null");
                return null;
            }
            var prefabToSpawn = ResourceLoader.LoadButton(type);
            if (prefabToSpawn == null)
            {
                return null;
            }

            GameObject buttonTransform = Instantiate(prefabToSpawn);
            if (buttonTransform == null)
            {
                Log.Error(MessageGroup.Gui, "Can't spawn button ( "+ type +" ) in popup.");
                return null;
            }

            buttonTransform.transform.parent = parent;
            buttonTransform.transform.localPosition = Vector3.zero;
            buttonTransform.transform.localEulerAngles = Vector3.zero;
            buttonTransform.transform.localScale = Vector3.one;

            Button button = buttonTransform.GetComponent<Button>();
            if (button == null)
            {
                Log.Error(MessageGroup.Gui, "Spawned prefab doesn't contain UIButton script. ( ");
                return null;
            }

            return button;
        }

        protected virtual void OnClickExitButton()
        {
            PopupManager.HidePopup(PopupType);
        }
    }
}