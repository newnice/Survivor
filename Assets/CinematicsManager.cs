using UnityEngine;

public class CinematicsManager : MonoBehaviour {
    [SerializeField] private RealtimeCinematicPlayer realtimePlayer = null;
    [SerializeField] private Canvas canvas = null;
    private PauseManager _pauseManager;

    private void Awake() {
        _pauseManager = FindObjectOfType<PauseManager>();
    }

    public void PlayRealtime() {
        OnStartCinematics(false);
        realtimePlayer.StartPlay(o => OnStopCinematics());
    }

    private void OnStopCinematics() {
        _pauseManager.ResumeObjects();
        canvas.enabled = true;
    }

    private void OnStartCinematics(bool isPauseMusic) {
        canvas.enabled = false;
        _pauseManager.PauseObjects(isPauseMusic);
    }

    protected virtual void Update() {
        if (!Input.GetKeyDown(KeyCode.Space) || !realtimePlayer.IsPlaying()) return;

        realtimePlayer.StopPlay();
        canvas.enabled = true;
    }
}