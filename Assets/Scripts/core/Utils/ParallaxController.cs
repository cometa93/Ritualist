using UnityEngine;
using System.Collections.Generic;

namespace DevilMind.Utils
{
    public class ParallaxController : DevilBehaviour
    {
        [SerializeField] private float _smoothFactor;
        [SerializeField] private GameObject _objectToFollow;
        [SerializeField] private List<ParallaxObject> _parallaxLayers = new List<ParallaxObject>();
        private Vector3 _previousPosition;
        
        protected override void Start()
        {
            _previousPosition = _objectToFollow.transform.position;
            for (int i = 0, c = _parallaxLayers.Count; i < c; ++i)
            {
                _parallaxLayers[i].SmoothFactor = _smoothFactor;
            }
        }

        //Calculating poses
        protected override void Update()
        {
            float prevX = _previousPosition.x;
            float currX = _objectToFollow.transform.position.x;
            float prevY = _previousPosition.y;
            float currY = _objectToFollow.transform.position.y;
            _previousPosition = _objectToFollow.transform.position;

            for (int i = 0, c = _parallaxLayers.Count; i < c; ++i)
            {
                _parallaxLayers[i].CalculateParallax(currX,prevX,currY,prevY);
                _parallaxLayers[i].CalculatePosition();
            }

            for (int i = 0, c = _parallaxLayers.Count; i < c; ++i)
            {
                _parallaxLayers[i].SetPosition();
            }
        }

        protected override void Reset()
        {
            _previousPosition = Vector3.zero;
        }
    }

}

