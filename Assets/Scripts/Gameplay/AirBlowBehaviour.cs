﻿using Ritualist;
using UnityEngine;

namespace Ritualist
{
    public class AirBlowBehaviour : MonoBehaviour
    {
        [Range(5, 40)] public float BlowPower;
        [SerializeField] private LayerMask _playerLayer;

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == _playerLayer.value)
            {
                var velo = collider.gameObject.GetComponent<Rigidbody2D>().velocity;
                velo.y = velo.y > 0 ? velo.y + BlowPower : BlowPower;
                collider.gameObject.GetComponent<Rigidbody2D>().velocity = velo;
                collider.gameObject.GetComponent<PlatformerCharacter2D>().DoubleJump = false;
            }   
        }
    }
}