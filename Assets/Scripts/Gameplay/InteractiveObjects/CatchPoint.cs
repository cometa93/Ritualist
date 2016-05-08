using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevilMind;
using DevilMind.Utils;
using Fading.AI;

namespace Fading
{
    public class CatchPoint : MonoBehaviour
    {
        [SerializeField] private const float TimeToTarget = 1.5f;

        [SerializeField] private LayerMask _collideAbleObjectsMask;
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private CircleCollider2D _collider2D;
        [SerializeField] private GameObject _particlesOnCollision;
        [SerializeField] private GameObject _destroyParticles;

        private bool _alreadyDocked;
        private Vector2 _target;

        private void FixedUpdate()
        {
            if (_alreadyDocked)
            {
                return;
            }

            Collider2D[] result = new Collider2D[1];
            if (Physics2D.OverlapCircleNonAlloc(transform.position, _collider2D.radius, result, _collideAbleObjectsMask) > 0)
            {
                iTween.Stop(gameObject, true);
                _alreadyDocked = true;
                Instantiate(_particlesOnCollision, transform.position, Quaternion.identity);
                StartCoroutine(TimeHelper.RunAfterSeconds(1f, DestroyMe));
                return;
            }
            
            if (Physics2D.OverlapCircleNonAlloc(transform.position, _collider2D.radius, result, _enemyLayerMask) > 0)
            {
                iTween.Stop(gameObject, true);
                _alreadyDocked = true;
                Instantiate(_particlesOnCollision, transform.position, Quaternion.identity);
                StartCoroutine(TimeHelper.RunAfterSeconds(1f, DestroyMe));
                MakeDamageToEnemy(result[0]);
            }
        }

        public void MakeDamageToEnemy(Collider2D result)
        {
            var enemy = result.GetComponent<AbstractEnemy>();
            if (enemy != null)
            {
                var skill = GameMaster.Hero.Skills[SkillEffect.Catch];
                if (skill == null)
                {
                    Log.Error(MessageGroup.Gameplay, "Can't retrieve skill from hero");
                    return;
                }
                enemy.Hit(skill.Damage);

            }
        }

        public void Shoot(Vector2 target)
        {
            iTween.MoveTo(gameObject, new Hashtable
            {
                { iT.MoveTo.orienttopath,true},
                { iT.MoveTo.path, CreateRandomPath(new Vector3(target.x, target.y,0)) },
                { iT.MoveTo.easetype, iTween.EaseType.easeInQuart},
                { iT.MoveTo.time, TimeToTarget},
                { iT.MoveTo.oncomplete, "DestroyMe"}
            });
        }

        private Transform[] CreateRandomPath(Vector3 target)
        {
                var direction = target - transform.position;
                direction.z = 0;
                var normal = direction.normalized;
                normal.z = 0;
                var crossed = Vector3.Cross(new Vector3(0,0,1), normal);
                Vector2 cross = crossed;
                var variance = direction.magnitude * 0.1f;

                var amount = 0.0f;
                var path = new List<Vector2>{ transform.position};

                while (amount < 1f)
                {
                    amount = Mathf.Clamp01(amount + Random.Range(0.2f, 0.3f));
                    var point = Vector2.Lerp(transform.position, target, amount);
                     point += cross * Random.Range(-variance, variance);
                    if (amount >= 1.0) point = target;
                    {
                        path.Add(point);
                    }
                }

            var transforms = new List<Transform>();
            foreach (var x in path)
            {
                var go = new GameObject("pathPoint");
                Destroy(go, TimeToTarget + 0.3f);
                go.transform.position = x;
                transforms.Add(go.transform);
            }
            return transforms.ToArray();
        }

        public void DestroyMe()
        {
            Instantiate(_destroyParticles, transform.position, Quaternion.identity);
            iTween.Stop(gameObject, true);
            Destroy(gameObject, 1f);
        }
    }
}