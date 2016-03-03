using DevilMind;
using UnityEngine;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Assets.Scripts.Gameplay.InteractiveObjects
{
    public class DealingDamageObject : DevilBehaviour
    {
        [SerializeField] private int Damage;
        [SerializeField] private float Cooldown;
        private float _cooldownCounter;
        private bool _canDealDamages = true;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.DamageByStaticObjectDone);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.DamageByStaticObjectDone)
            {
                _canDealDamages = false;
                _cooldownCounter = 0;
            }
            base.OnEvent(gameEvent);
        }

        protected override void Update()
        {
            if (_canDealDamages)
            {
                base.Update();
                return;
            }

            _cooldownCounter += Time.deltaTime;
            if (_cooldownCounter >= Cooldown)
            {
                _canDealDamages = true;
                _cooldownCounter = 0;
            }
            base.Update();
        }

        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (_canDealDamages == false)
            {
                return;
            }
            GameMaster.Hero.Stats.Power -= Damage;
            GameMaster.Events.Rise(EventType.DamageByStaticObjectDone);
        }
    }
}