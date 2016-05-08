using Fading;
using Fading.Controller;
using UnityEngine;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public class CatchableObjectActivationField : MonoBehaviour
    {
        private bool _isActivated;
        [SerializeField] private CatchAbleObject _parent;

        private void Awake()
        {
            gameObject.FadeTo(0.1f, Random.Range(2f, 4f), Random.Range(0f, 1f), LoopType.pingPong);
        }

        public bool IsActive
        {
            get { return _isActivated; }
            set
            {
                if (value == _isActivated)
                {
                    return;
                }

                _isActivated = value;
                _parent.ActivationFieldActivated();
            }
        }

        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            var goodSoulController = collider2D.GetComponent<GoodSoulController>();
            if (goodSoulController == null)
            {
                return;
            }

            iTween.Stop(gameObject);
            Color color = Color.green;
            color.a = 1;
            iTween.ColorTo(gameObject, color, 1.5f);
            IsActive = true;
        }
    }
}