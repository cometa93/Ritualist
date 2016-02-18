using UnityEngine;
using System.Collections;
using DevilMind;
using Event = DevilMind.Event;
using EventType = DevilMind.EventType;

namespace Ritualist.Controller
{
    public class RuneShooterBehaviour : DevilBehaviour
    {
        [SerializeField] private Transform _runePrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _worldParent;

        private RuneBehaviour _preparedRune;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.RightTriggerReleased);
            EventsToListen.Add(EventType.RightTriggerClicked);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.RightTriggerClicked)
            {
                PrepareRune();
            }
            if (gameEvent.Type == EventType.RightTriggerReleased)
            {
                FireRune();
            }
            base.OnEvent(gameEvent);
        }

        private void PrepareRune()
        {
            if (GameplayController.Instance.AmmoCount <= 0)
            {
                return;
            }

            Transform spawnedPrefab = Instantiate(_runePrefab, _spawnPoint.position, Quaternion.identity) as Transform;
            if (spawnedPrefab == null)
            {
                Debug.Log("Spawned rune is null");
                return;
            }

            ResourceLoader.SetLayer(spawnedPrefab, gameObject.layer);
            spawnedPrefab.transform.parent = transform;
            spawnedPrefab.localScale = Vector3.one;
            spawnedPrefab.localPosition = Vector3.zero;
            spawnedPrefab.localEulerAngles = Vector3.zero;

            _preparedRune = spawnedPrefab.GetComponent<RuneBehaviour>();
            if (_preparedRune == null)
            {
                Debug.LogWarning("rune dont have behaviour");
            }
        }

        private void FireRune()
        {
            if (GameplayController.Instance.AmmoCount <= 0)
            {
                return;
            }

            if (_preparedRune == null)
            {
                Debug.LogWarning("prepared rune is null");
                return;
            }
            GameplayController.Instance.AmmoCount--;

            _preparedRune._rigidbody2D.AddRelativeForce(new Vector2(0,140), ForceMode2D.Force);
            _preparedRune.transform.parent = _worldParent;
            _preparedRune = null;
        }


        protected override void Update()
        {
            if (_preparedRune != null)
            {
                _preparedRune.transform.localPosition = Vector3.zero;
                _preparedRune.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}
