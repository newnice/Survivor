using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nightmare {
    public class LevelManager : MonoBehaviour {
        [SerializeField] private List<string> levelNames = null;
        [SerializeField] private Text levelCompletedText = null;

        private int _currentLevel = -1;
        private Scene _currentScene;
        private CinematicsManager _cinematics;
        private AdsManager _adsManager;
        private PauseManager _pauseManager;
        private UnityAction<object> _onLevelComplete, _onRestartGame;

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.StartListening(NightmareEvent.LevelCompleted, _onLevelComplete);
            EventManager.StartListening(NightmareEvent.RestartGame, _onRestartGame);
            levelCompletedText.gameObject.SetActive(false);
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventManager.StopListening(NightmareEvent.LevelCompleted, _onLevelComplete);
            EventManager.StopListening(NightmareEvent.RestartGame, _onRestartGame);
        }

        private void Awake() {
            _cinematics = GetComponentInChildren<CinematicsManager>();
            _adsManager = FindObjectOfType<AdsManager>();
            _pauseManager = FindObjectOfType<PauseManager>();
            _onLevelComplete = o => StartCoroutine(nameof(LoadNextLevel));
            _onRestartGame = o => LoadLevel(0);
        }

        private void Start() {
            LoadLevel(0);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            SceneManager.SetActiveScene(scene);
            if (mode != LoadSceneMode.Additive) return;

            UnloadOldScene();
            _currentScene = scene;

            OnLevelLoaded();
        }

        private void OnLevelLoaded() {
            if (_currentLevel > 0) {
                _cinematics.PlayRealtime();
            }
        }

        private void UnloadOldScene() {
            if (_currentScene != null && _currentScene.IsValid())
                SceneManager.UnloadSceneAsync(_currentScene);
        }

        private void LoadLevel(int level) {
            _currentLevel = level;
            var levelToLoad = _currentLevel % levelNames.Count;
            SceneManager.LoadSceneAsync(levelNames[levelToLoad], LoadSceneMode.Additive);
            if (_currentLevel > 0) {
                _adsManager.ShowSimpleAds();
            }
        }

        private IEnumerator LoadNextLevel() {
            _pauseManager.PauseObjects(true);
            _currentLevel++;
            levelCompletedText.gameObject.SetActive(true);
            yield return new WaitForSeconds(5);
            
            levelCompletedText.gameObject.SetActive(false);
            LoadLevel(_currentLevel);
        }
    }
}