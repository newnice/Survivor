using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Nightmare {
    public class EventManager : MonoBehaviour {
        private Dictionary<NightmareEvent, UnityEvent> _events = new Dictionary<NightmareEvent, UnityEvent>();

        private static EventManager _manager;
        private EventManager() { }

        private static EventManager Instance {
            get {
                if (_manager == null) {
                    _manager = FindObjectOfType(typeof(EventManager)) as EventManager;
                    if (_manager == null)
                        Debug.LogError("Event manager not found!");
                }

                return _manager;
            }
        }


        public static void StartListening(NightmareEvent eventName, UnityAction action) {
            if (Instance == null) return;
            UnityEvent eventObject;
            if (!Instance._events.TryGetValue(eventName, out eventObject)) {
                eventObject = new UnityEvent();
                Instance._events.Add(eventName, eventObject);
            }

            eventObject.AddListener(action);
        }

        public static void RemoveListening(NightmareEvent eventName, UnityAction action) {
            if (Instance == null) return;
            if (Instance._events.TryGetValue(eventName, out var eventObject)) {
                eventObject.RemoveListener(action);
            }
        }
        
        public static void TriggerEvent<T>(NightmareEvent eventName, T parameters) {
            if (Instance == null) return;
            if (Instance._events.TryGetValue(eventName, out var eventObject)) {
                eventObject.Invoke();
            }
        }      
        public static void TriggerEvent(NightmareEvent eventName) {
            if (Instance == null) return;
            if (Instance._events.TryGetValue(eventName, out var eventObject)) {
                eventObject.Invoke();
            }
        }
    }
}