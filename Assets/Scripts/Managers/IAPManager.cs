#if UNITY_ANDROID
using System;
using Nightmare.UI;
using UnityEngine;
using UnityEngine.Purchasing;

#endif

namespace Nightmare {
    public class ProductId {
        internal const string Extra12Grenades = "ru.loonycreator.survivor.grenades";
        internal const string ExtraLife = "ru.loonycreator.survivor.extrahealth";
        internal const string GenerousFaeries = "ru.loonycreator.survivor.generousfaeries";
    }

    public interface IIAPService {
        bool IsInitialized();
        bool IsSubscriptionActive(string productId);
        void InitiatePurchase(string productId);
        bool IsProductBought(string productId);
    }

#if !UNITY_ANDROID
    public class IAPManager : IIAPService {
        public bool IsInitialized() {
            return true;
        }

        public bool IsSubscriptionActive(string productId) {
            return false;
        }

        public void InitiatePurchase(string productId) { }

        public bool IsProductBought(string productId) {
            return false;
        }
    }
#else
    public class IAPManager : MonoBehaviour, IStoreListener, IIAPService {
        [SerializeField] private StoreManager _storeManager = null;
        private IStoreController _controller;
        private IExtensionProvider _extensions;

        public bool clearPrefsOnStart = true;

        private void Start() {
            if (clearPrefsOnStart) {
                PlayerPrefs.DeleteAll();
            }
        }

        private void Awake() {
            Initialize();
        }

        private void Initialize() {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(ProductId.Extra12Grenades, ProductType.Consumable, new IDs {
                {ProductId.Extra12Grenades, GooglePlay.Name}
            });
            builder.AddProduct(ProductId.ExtraLife, ProductType.NonConsumable, new IDs {
                {ProductId.ExtraLife, GooglePlay.Name}
            });
            builder.AddProduct(ProductId.GenerousFaeries, ProductType.Subscription, new IDs {
                {ProductId.GenerousFaeries, GooglePlay.Name}
            });

            UnityPurchasing.Initialize(this, builder);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
            var product = e.purchasedProduct;
            switch (product.definition.id) {
                case ProductId.Extra12Grenades:
                    _storeManager.AfterGrenadesBought();
                    break;
                case ProductId.ExtraLife:
                    SaveNonConsumable(ProductId.ExtraLife);
                    _storeManager.AfterLifeBought();
                    break;
                case ProductId.GenerousFaeries:
                    SaveSubscription(product);
                    _storeManager.AfterFaeriesBought();
                    break;
                default:
                    Debug.LogError($"Unexpected product {product.definition.id} was purchased");
                    return PurchaseProcessingResult.Pending;
            }

            return PurchaseProcessingResult.Complete;
        }

        private void SaveSubscription(Product product) {
#if (!UNITY_EDITOR)
            var sm = new SubscriptionManager(product, null);
            var si = sm.getSubscriptionInfo();
            Debug.Log($"subscription product id = {si.getProductId()}");
            Debug.Log($"subscription expiration date = {si.getExpireDate()}");
            Debug.Log($"subscription isCancelled = {si.isCancelled()}");
            PlayerPrefs.SetString(product.definition.id, si.getExpireDate().Ticks.ToString());
#else
            PlayerPrefs.SetString(product.definition.id, DateTime.Now.AddMinutes(10).Ticks.ToString());
#endif
        }

        private void SaveNonConsumable(string productId) {
            PlayerPrefs.SetString(productId, true.ToString());
        }

        public bool IsProductBought(string productId) {
            return PlayerPrefs.HasKey(productId);
        }

        public bool IsSubscriptionActive(string productId) {
            if (!PlayerPrefs.HasKey(productId)) return false;

            var ticks = PlayerPrefs.GetString(productId);
            if (long.TryParse(ticks, out var endSubsTicks)) {
                return DateTime.Now.Ticks < endSubsTicks;
            }

            return false;
        }

        public void InitiatePurchase(string productId) {
            //development payload with "iapPromo" substring - strange fix for realtime
            //Error: called non-existent method System.Boolean UnityEngine.VR.VRSettings::get_enabled()) 

            _controller.InitiatePurchase(productId, $"iapPromo{DateTime.Now.Ticks}");
        }


        public void OnInitializeFailed(InitializationFailureReason error) {
            Debug.LogError($"IAP initialization failed with error {error}");
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
            Debug.LogError($"Product {i.definition.id} purchase failed because of {p}");
        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            _controller = controller;
            _extensions = extensions;
            Debug.Log("IAP was successfully initialized");
            foreach (var p in _controller.products.all) {
                Debug.Log($"Find product {p.definition.id} availableToPurchase = {p.availableToPurchase} " +
                          $"isEnabled = {p.definition.enabled} specStoreId={p.definition.storeSpecificId}");
            }
        }

        public bool IsInitialized() {
            return _controller != null;
        }
    }
#endif
}