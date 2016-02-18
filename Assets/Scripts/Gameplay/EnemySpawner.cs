using System.Collections.Generic;
using UnityEngine;

namespace DevilMind.Utils
{
    public class EnemySpawner : DevilBehaviour
    {
        [SerializeField] private List<GameObject> _spawners;

        protected override void Awake()
        {
            EventsToListen.Add(EventType.SpawnEnemy);
            base.Awake();
        }

        protected override void OnEvent(Event gameEvent)
        {
            if (gameEvent.Type == EventType.SpawnEnemy)
            {
                SpawnEnemy((EnemyType) gameEvent.Parameter);    
            }
            base.OnEvent(gameEvent);
        }

        private void SpawnEnemy(EnemyType type)
        {
            GameObject spawnPoint = _spawners[Random.Range(0, _spawners.Count - 1)];
            GameObject prefab = ResourceLoader.LoadEnemy(type);
            if (prefab == null)
            {
                return;
            }

            var spawned = Instantiate(prefab);
            ResourceLoader.SetLayer(spawned.transform, spawnPoint.gameObject.layer);
            spawned.transform.parent = transform;
            spawned.transform.localScale = Vector3.one;
            spawned.transform.position = spawnPoint.transform.position;
            spawned.transform.localEulerAngles = Vector3.zero;
        }
    }
}