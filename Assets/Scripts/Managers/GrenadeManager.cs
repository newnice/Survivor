using UnityEngine;
using UnityEngine.UI;

namespace Nightmare {
    public class GrenadeManager : MonoBehaviour {
        private int _grenades; // The player's grenades count.
        private Text _gText; // Reference to the Text component.

        void Awake() {
            // Set up the reference.
            _gText = GetComponent<Text>();
            // Reset the score.
            _grenades = 0;
        }

        protected virtual void OnEnable() {
            EventManager.StartListening(NightmareEvent.CollectGrenade, count => UpdateGrenadeCount((int) count));
            EventManager.StartListening(NightmareEvent.ShootGrenade, t => ShootGrenade((Transform) t));
            EventManager.StartListening(NightmareEvent.GameOver, o => UpdateGrenadeCount(-_grenades));
        }

        private void ShootGrenade(Transform t) {
            SharedPoolManager.Instance.Pull("Grenade", t.position, Quaternion.identity);
            UpdateGrenadeCount(-1);
        }


        protected virtual void OnDisable() {
            EventManager.StopListening(NightmareEvent.CollectGrenade, count => UpdateGrenadeCount((int) count));
            EventManager.StopListening(NightmareEvent.ShootGrenade, o => ShootGrenade((Transform) o));
            EventManager.StopListening(NightmareEvent.GameOver, o => UpdateGrenadeCount(-_grenades));
        }


        private void UpdateGrenadeCount(int diff) {
            _grenades += diff;
        }


        void Update() {
            _gText.text = $"Grenades: {_grenades}";
        }

        public bool HasGrenades() {
            return _grenades > 0;
        }
    }
}