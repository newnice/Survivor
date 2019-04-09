using UnityEngine;
using UnityEngine.Video;

public class CinematicsManager : MonoBehaviour {
    [SerializeField] private RealtimeCinematicPlayer realtimePlayer = null;
    [SerializeField] private Canvas canvas = null;
    private PauseManager _pauseManager;
    private VideoPlayer _videoPlayer;

    private void Awake() {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += o => {
            OnStopCinematics();
            _videoPlayer.Stop();
        };
        _pauseManager = FindObjectOfType<PauseManager>();
    }

    public void PlayRealtime() {
        OnStartCinematics(false);
        realtimePlayer.StartPlay(o=>OnStopCinematics());
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
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        realtimePlayer.StopPlay();
        _videoPlayer.Stop();
        canvas.enabled = true;
    }

    public void PlayStartCinematics() {
        OnStartCinematics(true);
        _videoPlayer.Play();
    }
}