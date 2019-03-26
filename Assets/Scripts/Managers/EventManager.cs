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


        private BaseEventManager<Vector3> _vectorManager = new BaseEventManager<Vector3>();
        private BaseEventManager<object> _voidManager = new BaseEventManager<object>();
        private BaseEventManager<bool> _boolManager = new BaseEventManager<bool>();
        private static EventManager _manager;
        private bool _isPaused = false;
        private EventManager() { }

        private static EventManager Instance {
            get {
                if (_manager == null) {
                    _manager = FindObjectOfType(typeof(EventManager)) as EventManager;
                }

                return _manager;
            }
        }


        public static void StartListening(NightmareEvent eventName, UnityAction action) {
            if (Instance == null) return;
            Instance._voidManager.StartListening(eventName, a => action.Invoke());
        }

        public static void StartListening(NightmareEvent eventName, UnityAction<Vector3> action) {
            if (Instance == null) return;
            Instance._vectorManager.StartListening(eventName, action);
        }       
        
        public static void StartListening(NightmareEvent eventName, UnityAction<bool> action) {
            if (Instance == null) return;
            Instance._boolManager.StartListening(eventName, action);
        }

        public static void StopListening(NightmareEvent eventName, UnityAction<Vector3> action) {
            if (Instance == null) return;
            Instance._vectorManager.RemoveListening(eventName, action);
        }

        public static void StopListening(NightmareEvent eventName, UnityAction action) {
            if (Instance == null) return;
            Instance._voidManager.RemoveListening(eventName, a => action.Invoke());
        }
        public static void StopListening(NightmareEvent eventName, UnityAction<bool> action) {
            if (Instance == null) return;
            Instance._boolManager.RemoveListening(eventName, action);
        }

        public static void TriggerEvent(NightmareEvent eventName) {
            if (Instance == null) return;
            Instance._voidManager.TriggerEvent(eventName, null);
        }


        public static void TriggerEvent(NightmareEvent eventName, Vector3 parameters) {
            if (Instance == null) return;
            Instance._vectorManager.TriggerEvent(eventName, parameters);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                _isPaused = !_isPaused;
                Instance._boolManager.TriggerEvent(NightmareEvent.PauseGame, _isPaused);
            }
        }
    }
}