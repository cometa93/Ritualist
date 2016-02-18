using System.Collections.Generic;

namespace DevilMind.EventSystem
{
    public class EventSystem
    {
        public delegate void EventDelegate(Event eventObj);
         
        private readonly Dictionary<EventType,List<EventDelegate>> _delegates = new Dictionary<EventType, List<EventDelegate>>();

        public void AddListener(EventType type, EventDelegate eventDelegate)
        {
            if (eventDelegate == null)
            {
                Log.Error(MessageGroup.Common, "Tried to add nulled listener into _delegates list");
            }
            if (!_delegates.ContainsKey(type))
            {
                _delegates.Add(type,new List<EventDelegate>());
            }
            if (!_delegates[type].Contains(eventDelegate))
            {
                 _delegates[type].Add(eventDelegate);
            }
            else
            {
                Log.Info(MessageGroup.Common, "This delegate is already in the list.");
            }
        }

        public void RemoveListener(EventType eventType, EventDelegate eventDelegate)
        {
            if (_delegates.ContainsKey(eventType) == false)
            {
                return;
            }
            _delegates[eventType].Remove(eventDelegate);
        }

        public void Rise(EventType type, object parameter = null, object sender = null)
        {
            Log.Info(MessageGroup.Common, "EVENT RISED: " + type);
            Event eventParam = new Event()
            {
                Type = type,
                Grantor = sender,
                Parameter = parameter
            };
            if (_delegates.ContainsKey(type) == false)
            {
                return;
            }
            
            for (int i = 0, c = _delegates[type].Count; i < c; ++i)
            {
                _delegates[type][i](eventParam);
            }
        }
    
    }
}