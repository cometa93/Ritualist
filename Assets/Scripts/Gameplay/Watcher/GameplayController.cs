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
        private GameObject _magicFieldPrefab;
        private MagicField.Config _config;
        
        private static GameplayController _instance;
        public static GameplayController Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("GameplayController");
                    obj.transform.position = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localEulerAngles = Vector3.zero;
                    obj.AddComponent<GameplayController>();
                    _instance = obj.GetComponent<GameplayController>();
                }
                return _instance;
            }
        }

        protected override void Awake()
        {
            GameMaster.Hero.Stats.Reset();
            _magicFieldPrefab = ResourceLoader.LoadMagicField();
            if (_magicFieldPrefab == null)
            {
                Log.Error(MessageGroup.Gameplay, "MagicField prefab is null");
            }
            base.Awake();
        }

        #region Catch Skill Helper
        public void PlacePoint(CatchPoint point)
        {
            var config = _config ?? ( _config = new MagicField.Config()
            {
                LongLife = GameMaster.Hero.Skills[SkillEffect.Catch].LongLife,
                CatchPoints = new List<CatchPoint>()
            });

            config.CatchPoints.Add(point);

            if (config.IsFull)
            {
                _config = null;
                var magicField = Instantiate(_magicFieldPrefab);
                var goPosition = magicField.transform.position;
                goPosition.z = 15;
                magicField.transform.position = goPosition;
                StartCoroutine(TimeHelper.RunAfterFrames(5, () =>
                {
                    magicField.GetComponent<MagicField>().Setup(config);
                }));
            }
        }

        #endregion
    }
}
