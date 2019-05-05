using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nightmare {
    public class UINotificationHelper : MonoBehaviour {
        [SerializeField] private Text informationText = null;


        private IEnumerator StartInform(Action onInformCompleted) {
            informationText.gameObject.SetActive(true);
            var timeScale = Time.timeScale;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(2);

            informationText.gameObject.SetActive(false);
            onInformCompleted?.Invoke();
            Time.timeScale = timeScale;
        }

        public void Inform(string message) {
            Inform(message, null);
        }
        public void Inform(string message, Action onInformCompleted) {
            informationText.text = message;
            StartCoroutine(StartInform(onInformCompleted));
        }
    }
}