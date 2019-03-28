using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nightmare {
    public class LevelManager : MonoBehaviour {
        [SerializeField] private List<string> levelNames = null;

        private int _currentLevel = -1;
        private Scene _currentScene;

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.StartListening(NightmareEvent.LevelCompleted, o=>LoadNextLevel());
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventManager.StopListening(NightmareEvent.LevelCompleted, o=>LoadNextLevel());
        }

        private void Start() {
            LoadNextLevel();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            SceneManager.SetActiveScene(scene);

            if (mode == LoadSceneMode.Additive) {
                UnloadOldScene();
                _currentScene = scene;
            }
        }

        private void UnloadOldScene() {
            if (_currentScene != null && _currentScene.IsValid())
                SceneManager.UnloadSceneAsync(_currentScene);
        }

        private void LoadNextLevel() {
            if (levelNames.Count == _currentLevel + 1) {
                _currentLevel = 0;
            }
            else {
                _currentLevel++;
            }

            SceneManager.LoadSceneAsync(levelNames[_currentLevel], LoadSceneMode.Additive);
        }
    }
}