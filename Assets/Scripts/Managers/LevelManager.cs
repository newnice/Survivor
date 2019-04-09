using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nightmare {
    public class LevelManager : MonoBehaviour {
        [SerializeField] private List<string> levelNames = null;

        private int _currentLevel = -1;
        private Scene _currentScene;
        private CinematicsManager _cinematics;

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.StartListening(NightmareEvent.LevelCompleted, o => LoadNextLevel());
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventManager.StopListening(NightmareEvent.LevelCompleted, o => LoadNextLevel());
        }

        private void Awake() {
            _cinematics = GetComponentInChildren<CinematicsManager>();
        }

        private void Start() {
            LoadNextLevel();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            SceneManager.SetActiveScene(scene);
            if (mode != LoadSceneMode.Additive) return;

            UnloadOldScene();
            _currentScene = scene;

            PlayCinematics();
        }

        private void PlayCinematics() {
            if (_currentLevel > 0)
                _cinematics.PlayRealtime();
            else
                _cinematics.PlayStartCinematics();
        }

        private void UnloadOldScene() {
            if (_currentScene != null && _currentScene.IsValid())
                SceneManager.UnloadSceneAsync(_currentScene);
        }

        private void LoadNextLevel() {
            _currentLevel++;
            var levelToLoad = _currentLevel % levelNames.Count;
            SceneManager.LoadSceneAsync(levelNames[levelToLoad], LoadSceneMode.Additive);
        }
    }
}