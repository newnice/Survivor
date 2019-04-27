using UnityEngine;
using UnityEngine.Advertisements;

namespace Nightmare {
    public class AdsManager : MonoBehaviour {
        [SerializeField] private string gameId = "";

        [SerializeField] private bool testMode = false;

        void Start() {
            Advertisement.Initialize(gameId, testMode);
        }


        public void ShowAds() {
            if (Advertisement.IsReady())
                Advertisement.Show();
        }
    }
}