using UnityEngine;
using UnityEngine.UI;

namespace Nightmare {
    public class ScoreManager : MonoBehaviour {
        [SerializeField] private int scoreStepToCompleteLevel = 1000;
        private int _score;
        private int _maxLevelScore;

        private Text _sText;

        void Awake() {
            _sText = GetComponent<Text>();
            _score = 0;
            _maxLevelScore = scoreStepToCompleteLevel;
        }

        private void ResetScore() {
            _score = 0;
        }

        private void OnEnable() {
            EventManager.StartListening(NightmareEvent.GameOver, o=>ResetScore());
            EventManager.StartListening(NightmareEvent.EnemyKilled, s=>UpdateScore((int) s));
        }

        private void OnDestroy() {
            EventManager.StopListening(NightmareEvent.GameOver, o=>ResetScore());
            EventManager.StopListening(NightmareEvent.EnemyKilled, s=>UpdateScore((int) s));
        }

        private void UpdateScore(int value) {
            _score += value;
            if (_score >= _maxLevelScore) {
                _maxLevelScore += scoreStepToCompleteLevel;
                EventManager.TriggerEvent(NightmareEvent.LevelCompleted, null);
            }
        }

        void Update() {
            _sText.text = $"Score: {_score}";
        }
    }
}