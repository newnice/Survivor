using Nightmare;
using UnityEngine;

public class CinematicsManager : MonoBehaviour {
    [SerializeField] private RealtimeCinematicPlayer realtimePlayer = null;


    public void PlayRealtime() {
        EventManager.TriggerEvent(NightmareEvent.PauseGame, true);
        realtimePlayer.StartPlay(o => EventManager.TriggerEvent(NightmareEvent.PauseGame, false));
    }

    protected virtual void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            realtimePlayer.StopPlay();
        }
    }

    public void PlayStartCinematics() {
        Debug.Log("start prerendered cinematics");
    }
}