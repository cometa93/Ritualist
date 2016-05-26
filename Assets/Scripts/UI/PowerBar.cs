using DevilMind;
using UnityEngine;
using UnityEngine.UI;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Fading.UI
{
    public class PowerBar : DevilBehaviour
    {
        [SerializeField] private Image _progressImage;
        [SerializeField] private RectTransform _imageTransform;
        private float _currentRotationTime = 0;
        private float _timeTillFullRotation = 10f;

        private float Value
        {
            get { return (float) GameMaster.Hero.Stats.Power/GameMaster.Hero.Stats.MaxPower; }
        }

        protected override void Awake()
        {
            EventsToListen.Add(EventType.HeroPowerChanged);
            SetupBar();
            base.Awake();
        }

        protected override void Update()
        {
            _currentRotationTime += Time.unscaledDeltaTime;
            float progress = _currentRotationTime/_timeTillFullRotation;
            if (progress >= 1)
            {
                _currentRotationTime -= _timeTillFullRotation;
            }

            float zRotation = Mathf.Lerp(0, 360, progress);
            
            var temp = _imageTransform.localRotation.eulerAngles;
            temp.z = zRotation;
            _imageTransform.rotation = Quaternion.Euler(temp);
            base.Update();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.HeroPowerChanged)
            {
                SetupBar();
            }
            base.OnEvent(gameEvent);
        }

        private void SetupBar()
        {
            _progressImage.fillAmount = Value;
        }
    }
}