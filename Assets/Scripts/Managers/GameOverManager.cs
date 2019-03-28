using UnityEngine;

namespace Nightmare {
    public class GameOverManager : MonoBehaviour {
        private Animator _anim;

        void Awake() {
            _anim = GetComponent<Animator>();
        }

        private void OnEnable() {
            EventManager.StartListening(NightmareEvent.GameOver, o=>ShowGameOver());
        }

        private void OnDestroy() {
            EventManager.StopListening(NightmareEvent.GameOver, o=>ShowGameOver());
        }

        private void ShowGameOver() {
            _anim.SetBool(AnimationConstants.GameOverAttribute, true);
        }

        /**
         * Function for animation GameOverClip completed
         */
        private void ResetLevel() {
            _anim.SetBool(AnimationConstants.GameOverAttribute, false);
            EventManager.TriggerEvent(NightmareEvent.ResetLevel);
        }
    }
}