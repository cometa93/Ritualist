﻿using Fading;
using UnityEngine;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public class CatchPointTarget : SkillTarget
    {
        private bool _isActivated;
        [SerializeField] private CatchAbleObject _parent;

        protected override void Awake()
        {
            base.Awake();
            gameObject.FadeTo( 0.2f, 2f, Random.Range(0.5f,1.5f), LoopType.pingPong);
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

        protected override void OnTriggerEnter2D(Collider2D collider2D)
        {
            var catchPoint = collider2D.GetComponent<CatchPoint>();
            if (catchPoint == null)
            {
                return;
            }

            IsActive = true;
            base.OnTriggerEnter2D(collider2D);
        }
    }
}