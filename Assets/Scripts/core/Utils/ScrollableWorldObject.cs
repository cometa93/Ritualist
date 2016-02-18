using DevilMind;
using UnityEngine;

namespace Assets.Scripts.core.Utils
{
    public class ScrollableWorldObject : DevilBehaviour
    {
        private Vector3 _startingPosition;
        private Vector3 _nextPosition;
        public float UnitsPerTime;

        protected override void Start()
        {
            base.Start();
            _startingPosition = transform.position;
            _nextPosition = new Vector3(_startingPosition.x + UnitsPerTime, _startingPosition.y, _startingPosition.z);
        }

        public void Scroll(float progress)
        {
            transform.position = Vector3.Lerp(_startingPosition, _nextPosition, progress);
        }

        public void SetNextPosition()
        {   
            _startingPosition = _nextPosition;
            _nextPosition = new Vector3(_startingPosition.x + UnitsPerTime, _startingPosition.y, _startingPosition.z);
        }

        protected override void Reset()
        {
        }
    }
}