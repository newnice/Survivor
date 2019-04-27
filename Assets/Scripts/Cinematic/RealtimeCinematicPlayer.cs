using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class RealtimeCinematicPlayer : MonoBehaviour {
    private CinemachineVirtualCamera _cinematicCamera;
    private PlayableDirector _director;


    void Awake() {
        _cinematicCamera = GetComponent<CinemachineVirtualCamera>();
        _director = GetComponent<PlayableDirector>();
        _director.stopped += o => DisableCamera();
        _director.played += o => ActivateCamera();
    }

    private void ActivateCamera() {
        _cinematicCamera.Priority = 100;
    }

    private void DisableCamera() {
        _cinematicCamera.Priority = 10;
    }

    public void StartPlay(Action<PlayableDirector> onStopCinematic) {
        _director.stopped +=  onStopCinematic;
        _director.Play();
    }

    public void StopPlay() {
        _director.Stop();
    }

    public bool IsPlaying() {
        return _director.state == PlayState.Playing;
    }
}