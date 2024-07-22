using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    Dictionary<string, UnityEngine.Events.UnityEvent> _events = new Dictionary<string, UnityEngine.Events.UnityEvent>();
    public EventAnim[] events;

    [System.Serializable]
    public struct EventAnim {
        public string name;
        public UnityEngine.Events.UnityEvent @event;
    }

	private void Awake()
	{
		foreach(EventAnim item in events){
            _events.Add(item.name, item.@event);
        }
	}

	public void Event(string name){
        UnityEngine.Events.UnityEvent @event;
        if (_events.TryGetValue(name,out @event)){
            @event.Invoke();
		}
    }
    public UnityEngine.Events.UnityEvent Get(string name){
        return _events[name];
    }
}
