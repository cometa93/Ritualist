using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
using EventType = DevilMind.EventType;

namespace Ritualist
{
    public class GameplayController : DevilBehaviour
    {
        public bool GameEnded { get; private set; }
        public int EnemyCounter { get; set; }
        private const int MaximalRunesCount = 3;
        public int EnemyKilledCounter = 0;

        private static GameplayController _instance;
        public static GameplayController Instance {  get { return _instance; } }

        protected override void Awake()
        {
            GameMaster.Hero.Stats.Reset();
            _instance = null;
            _instance = this;
            SpawnEnemy(EnemyType.Ghost);
            base.Awake();
        }

        private List<RuneBehaviour> _placedRunes = new List<RuneBehaviour>();
        public int AmmoCount = MaximalRunesCount;

        public void PlaceRune(RuneBehaviour behaviour)
        {
            _placedRunes.Add(behaviour);
            if (_placedRunes.Count == MaximalRunesCount)
            {
                GenerateMagicField();
            }
        }

        private void GenerateMagicField()
        {
            GameMaster.Events.Rise(EventType.GenerateMagicField, new List<RuneBehaviour>(_placedRunes));
            _placedRunes.Clear();
        }

        public void SpawnEnemy(EnemyType enemyType)
        {
            StartCoroutine(SpawnEnemyLater(enemyType));
        }

        private IEnumerator SpawnEnemyLater(EnemyType type)
        {
            yield return new WaitForSeconds(Random.Range(0,2f));
            GameMaster.Events.Rise(EventType.SpawnEnemy,type);
        }

        public void RefillAmmo()
        {
            AmmoCount = MaximalRunesCount;
        }

        #region Actions performed on hero.

        public void PerformAttackOnHero(int damage)
        {
            GameMaster.Hero.Stats.Health -= damage;
            GameMaster.Events.Rise(EventType.HeroHurted);

            if (GameMaster.Hero.Stats.Health <= 0)
            {
                GameEnded = true;
                GameMaster.Events.Rise(EventType.GameEnd);
            }
        }

        #endregion
    }
}
