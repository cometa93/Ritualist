using UnityEngine;

namespace DevilMind.Utils
{
    public class ParallaxObject : DevilBehaviour
    {
        [SerializeField] private float _speedFactor;
        [SerializeField] private bool _parallaxAxisY;

        private float _parallaxX,_parallaxY;
        private float _prevX, _currX;
        private float _prevY, _currY;
        private float _smoothFactor;

        protected override void Start()
        {
            _currX = transform.position.x;
            _currY = transform.position.y;
            _speedFactor = _speedFactor*-1;
        }

        public float SmoothFactor
        {
            set
            {
                if (_smoothFactor <= 0)
                {
                    _smoothFactor = value;
                }
            }
            get
            {
                return _smoothFactor;
            }
        }

        public void CalculateParallax(float x, float prevX,float y,float prevY)
        {
            _parallaxX = (x - prevX)*_speedFactor;
            _parallaxY = _parallaxAxisY ? (y - prevY)*_speedFactor : 0;
        }

        public void CalculatePosition()
        {
            _prevX = _currX;
            _currX = _prevX + _parallaxX;
            if (!_parallaxAxisY) return;
            _prevY = _currY;
            _currY = _prevY + _parallaxY;
        }

        public void SetPosition()
        {
            Vector3 _temp = transform.position;
            _temp.x = _currX;
            if (_parallaxAxisY)
            {
                _temp.y = _currY;
            }
            transform.position = Vector3.Lerp(transform.position, _temp, _smoothFactor*Time.deltaTime);
        }

        protected override void Reset()
        {
            if (enabled)
            {
                _prevX = 0;
                _prevY = 0;
                _currX = 0;
                _currY = 0;
                _smoothFactor = 0f;
                _parallaxX = 0;
                _parallaxY = 0;
            }
        }
    }

}
