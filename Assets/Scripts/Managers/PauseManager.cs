using Nightmare;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class PauseManager : MonoBehaviour {
    [SerializeField] private AudioMixerSnapshot paused = null;
    [SerializeField] private AudioMixerSnapshot unpaused = null;
    [SerializeField] private Canvas mainCanvas = null;
    [SerializeField] private Canvas pausedCanvas = null;
    [SerializeField] private AudioSource musicSource = null;

    public bool _isPausedObjects, _isPausedGame;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !_isPausedGame) {
            _isPausedObjects = !_isPausedObjects;
            if (_isPausedObjects)
                PauseObjects(false);
            else
                ResumeObjects();
        }

        if (Input.GetKeyDown(KeyCode.Escape)&& !_isPausedObjects) {
            PauseGame(!_isPausedGame);
        }
    }


    public void PauseObjects(bool isStopMusic) {
        _isPausedObjects = true;
        EventManager.TriggerEvent(NightmareEvent.PauseObjects, true);
        musicSource.enabled = !isStopMusic;
    }

    public void ResumeObjects() {
        _isPausedObjects = false;
        EventManager.TriggerEvent(NightmareEvent.PauseObjects, false);
        musicSource.enabled = true;
    }


    /**
     * Invoked by UI Pause button
     */
    private void PauseGame() {
        mainCanvas.enabled = !_isPausedGame;
        pausedCanvas.enabled = _isPausedGame;
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Lowpass();
    }

    public void PauseGame(bool isPause) {
        _isPausedGame = isPause;
        PauseGame();
    }

    private void Lowpass() {
        if (Time.timeScale == 0) {
            paused.TransitionTo(.01f);
        }
        else {
            unpaused.TransitionTo(.01f);
        }
    }

/**
     * Invoked by UI Quit button
     */
    public void Quit() {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}