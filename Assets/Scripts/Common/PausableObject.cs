using UnityEngine;

namespace Nightmare {
    public abstract class PausableObject: MonoBehaviour {
        private bool _isPaused;
        protected bool IsPausedGame => _isPaused;
        protected virtual void OnEnable() {
            EventManager.StartListening(NightmareEvent.PauseObjects, i=>OnPause((bool)i));
        }

        protected virtual void OnPause(bool isPaused) {
            _isPaused = isPaused;
        }
        
        protected virtual void OnDisable() {
            EventManager.StopListening(NightmareEvent.PauseObjects, i=>OnPause((bool)i));
        }
    }
}