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
            EventManager.StartListening(NightmareEvent.CollectGrenade, o=>IncrementGrenades());
            EventManager.StartListening(NightmareEvent.ShootGrenade, o=>DecrementGrenades());
            EventManager.StartListening(NightmareEvent.RestartGame, o=>UpdateCount(-_grenades));
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
            EventManager.StopListening(NightmareEvent.RestartGame, o=>UpdateCount(-_grenades));
        }


        private void UpdateCount(int diff) {
            _grenades += diff;
        }


        void Update() {
            _gText.text = $"Grenades: {_grenades}";
        }
    }
}