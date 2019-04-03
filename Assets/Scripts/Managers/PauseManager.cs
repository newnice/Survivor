using Nightmare;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class PauseManager : MonoBehaviour {
    [SerializeField] private AudioMixerSnapshot paused = null;
    [SerializeField] private AudioMixerSnapshot unpaused = null;

    private bool _isPaused;
    private Canvas canvas;

    private void Start() {
        canvas = GetComponent<Canvas>();
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)) {
            _isPaused = !_isPaused;
            EventManager.TriggerEvent(NightmareEvent.PauseGame, _isPaused);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            canvas.enabled = !canvas.enabled;
            Pause();
        }
    }

    private void Pause() {
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