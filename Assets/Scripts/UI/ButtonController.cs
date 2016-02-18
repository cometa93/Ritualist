using DevilMind;
using DevilMind.Utils;
using UnityEngine;
using EventType = DevilMind.EventType;

namespace Ritualist.UI
{
    public class ButtonController : MonoBehaviour
    {
        public void OnButtonClick()
        {
            GameMaster.Events.Rise(EventType.SpawnEnemy,EnemyType.Ghost);
        }
    }
}