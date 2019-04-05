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

    private bool _isPaused;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)) {
            _isPaused = !_isPaused;
            EventManager.TriggerEvent(NightmareEvent.PauseGame, _isPaused);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            mainCanvas.enabled = !_isPaused;
            pausedCanvas.enabled = _isPaused;
            PauseGame();
        }
    }


    public void PauseObjects() {
        _isPaused = true;
        EventManager.TriggerEvent(NightmareEvent.PauseGame, true);
    }

    public void ResumeObjects() {
        _isPaused = false;
        EventManager.TriggerEvent(NightmareEvent.PauseGame, false);
    }

    private void PauseGame() {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Lowpass();
    }

    private void Lowpass() {
        if (Time.timeScale == 0) {
            paused.TransitionTo(.01f);
        }

        else {
            unpaused.TransitionTo(.01f);
        }
    }

    public void Quit() {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}