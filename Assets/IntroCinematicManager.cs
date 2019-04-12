using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroCinematicManager : MonoBehaviour {
    private VideoPlayer _videoPlayer;
    private AsyncOperation _mainSceneLoad;

    private void Awake() {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += o => OnStopIntro();
        StartCoroutine("LoadMainScene");
    }

    protected virtual void Update() {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        OnStopIntro();
    }


    IEnumerator LoadMainScene() {
        yield return null;

        _mainSceneLoad = SceneManager.LoadSceneAsync(1);
        _mainSceneLoad.allowSceneActivation = false;
        while (!_mainSceneLoad.allowSceneActivation) {
            yield return null;
        }
    }


    private void OnStopIntro() {
        _mainSceneLoad.allowSceneActivation = true;
        _videoPlayer.Stop();
    }
}