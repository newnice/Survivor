using UnityEngine;
using UnityEngine.UI;

namespace Nightmare {
    public class GrenadeManager : MonoBehaviour {
        private int _grenades; // The player's score.
        private Text _gText; // Reference to the Text component.


        void Awake() {
            // Set up the reference.
            _gText = GetComponent<Text>();
            // Reset the score.
            _grenades = 0;
        }

        protected virtual void OnEnable() {
            EventManager.StartListening(NightmareEvent.CollectGrenade, o=>IncrementGrenades());
            EventManager.StartListening(NightmareEvent.ShootGrenade, o=>DecrementGrenades());
        }

        private void DecrementGrenades() {
            UpdateCount(-1);
        }

        private void IncrementGrenades() {
            UpdateCount(1);
        }

        protected virtual void OnDisable() {
            EventManager.StopListening(NightmareEvent.CollectGrenade, o=>IncrementGrenades());
            EventManager.StopListening(NightmareEvent.ShootGrenade, o=>DecrementGrenades());
        }


        private void UpdateCount(int diff) {
            _grenades += diff;
        }


        void Update() {
            _gText.text = $"Grenades: {_grenades}";
        }
    }
}