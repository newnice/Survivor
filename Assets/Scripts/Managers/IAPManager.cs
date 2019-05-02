using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener {
    private void Start() {
       // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
       // builder.AddProduct();
    }

    public void OnInitializeFailed(InitializationFailureReason error) {
        throw new System.NotImplementedException();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
        throw new System.NotImplementedException();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        throw new System.NotImplementedException();
    }
}