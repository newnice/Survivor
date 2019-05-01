using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Nightmare {
    public class AdsManager : MonoBehaviour {
        [SerializeField] private string gameId = "";
        [SerializeField] private bool testMode = false;
        [SerializeField] private Button adsForGrenadesButton = null;

        private float _timer = 0f;

        void Awake() {
            Advertisement.Initialize(gameId, testMode);
            adsForGrenadesButton.onClick.AddListener(() => {
                _timer = 0;
                ShowRewardedAds(() =>
                    EventManager.TriggerEvent(NightmareEvent.CollectGrenade, 3));
            });
        }


        public void ShowSimpleAds() {
            ShowAds(null);
        }

        private void ShowAds(ShowOptions options) {
            if (!Advertisement.IsReady()) return;
            Advertisement.Show(options);
        }

        public void OfferAdsForGrenades() {
            if (_timer > 0) return;
            adsForGrenadesButton.gameObject.SetActive(true);
            _timer = 10f;
        }

        private void ShowRewardedAds(Action reward) {
            var so = new ShowOptions {
                resultCallback = delegate(ShowResult result) {
                    switch (result) {
                        case ShowResult.Finished:
                            reward.Invoke();
                            break;
                        case ShowResult.Skipped:
                            Debug.Log("Oops, ads was skipped, no reward(");
                            break;
                        default:
                            Debug.Log("Oops, something went wrong... no reward(");
                            break;
                    }
                }
            };

            ShowAds(so);
        }


        private void Update() {
            if (_timer < 0) return;

            _timer -= Time.deltaTime;
            adsForGrenadesButton.gameObject.SetActive(_timer > 0);
        }
    }
}