using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Nightmare {
    public class EventManager : MonoBehaviour {
        private class BaseEventManager<T> {
            private class CustomEvent : UnityEvent<T> { }

            private Dictionary<NightmareEvent, UnityEvent<T>> _events = new Dictionary<NightmareEvent, UnityEvent<T>>();

            protected internal void StartListening(NightmareEvent eventName, UnityAction<T> action) {
                UnityEvent<T> eventObject;
                if (!_events.TryGetValue(eventName, out eventObject)) {
                    eventObject = new CustomEvent();
                    _events.Add(eventName, eventObject);
                }

                eventObject.AddListener(action);
            }


            protected internal void RemoveListening(NightmareEvent eventName, UnityAction<T> action) {
                if (_events.TryGetValue(eventName, out var eventObject)) {
                    eventObject.RemoveListener(action);
                }
            }

            protected internal void TriggerEvent(NightmareEvent eventName, T parameters) {
                if (_events.TryGetValue(eventName, out var eventObject)) {
                    eventObject.Invoke(parameters);
                }
            }
        }

        private BaseEventManager<object> _objectManager = new BaseEventManager<object>();
        
        private static EventManager _manager;
        private EventManager() { }

        private static EventManager Instance {
            get {
                if (_manager == null) {
                    _manager = FindObjectOfType(typeof(EventManager)) as EventManager;
                }

                return _manager;
            }
        }

        
        public static void StartListening(NightmareEvent eventName, UnityAction<object> action) {
            if (Instance == null) return;
            Instance._objectManager.StartListening(eventName, action);
        }

        public static void StopListening(NightmareEvent eventName, UnityAction<object> action) {
            if (Instance == null) return;
            Instance._objectManager.RemoveListening(eventName, action);
        }
     
        public static void TriggerEvent(NightmareEvent eventName, object parameters) {
            if (Instance == null) return;
            Instance._objectManager.TriggerEvent(eventName, parameters);
        }

        public static void TriggerEvent(NightmareEvent eventName) {
            TriggerEvent(eventName, null);
        }
        
    }
}