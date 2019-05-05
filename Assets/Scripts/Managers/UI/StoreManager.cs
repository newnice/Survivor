using UnityEngine;
using UnityEngine.UI;

namespace Nightmare.UI {
    public class StoreManager : MonoBehaviour {
        [SerializeField] private IIAPService iapManager = null;
        private UINotificationHelper _uiNotificationHelper;
        [SerializeField] private Button extraLifeButton = null;
        [SerializeField] private Button generousFaeriesButton = null;
        [SerializeField] private Button grenadesButton = null;
        [SerializeField] private Text storeNotWorkingText = null;


        private void Start() {
            _uiNotificationHelper = FindObjectOfType<UINotificationHelper>();
            CheckPreviousPurchases();
        }

        private void Update() {
            if (!iapManager.IsInitialized()) return;

            generousFaeriesButton.enabled = !iapManager.IsSubscriptionActive(ProductId.GenerousFaeries);
        }

        public void OnBuyGrenades() {
            iapManager.InitiatePurchase(ProductId.Extra12Grenades);
        }

        public void OnBuyLife() {
            iapManager.InitiatePurchase(ProductId.ExtraLife);
        }

        public void OnBuyFaerie() {
            iapManager.InitiatePurchase(ProductId.GenerousFaeries);
        }

        private void CheckPreviousPurchases() {
            if (!iapManager.IsInitialized()) {
                extraLifeButton.gameObject.SetActive(false);
                generousFaeriesButton.gameObject.SetActive(false);
                grenadesButton.gameObject.SetActive(false);
                storeNotWorkingText.gameObject.SetActive(true);
                return;
            }

            if (iapManager.IsProductBought(ProductId.ExtraLife)) {
                EventManager.TriggerEvent(NightmareEvent.HealthIncreased, 1.1f);
                extraLifeButton.gameObject.SetActive(false);
            }

            generousFaeriesButton.enabled = !iapManager.IsSubscriptionActive(ProductId.GenerousFaeries);
        }

        public void AfterGrenadesBought() {
            EventManager.TriggerEvent(NightmareEvent.CollectGrenade, 12);
        }
        
        public bool IsFaeriesGenerous() {
            return iapManager.IsInitialized() && iapManager.IsSubscriptionActive(ProductId.GenerousFaeries);
        }

        public void AfterLifeBought() {
            EventManager.TriggerEvent(NightmareEvent.HealthIncreased, 1.1f);
            _uiNotificationHelper.Inform("Health was increased on 10%");
            extraLifeButton.gameObject.SetActive(false);
        }

        public void AfterFaeriesBought() {
            EventManager.TriggerEvent(NightmareEvent.GenerousFaeriesActivated, true);
            _uiNotificationHelper.Inform("Faeries are very generous now!");
            generousFaeriesButton.enabled = false;
        }
    }
}