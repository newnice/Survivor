using UnityEngine;

public class AssetBundleLoader : MonoBehaviour {
    public static GameObject LoadAsset(string assetBundleName, string assetName) {

        var path = $"{Application.streamingAssetsPath}/{assetBundleName}";
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(path);
  
        if (myLoadedAssetBundle == null) {
            Debug.Log($"Failed to load AssetBundle {assetBundleName}!");
            return null;
        }

        if (assetName != null) {
            var assetObject = myLoadedAssetBundle.LoadAsset<GameObject>(assetName);
            return assetObject;
        }

        
        myLoadedAssetBundle.LoadAllAssets();
        return null;
    }

    public static void ClearLoadCache() {
        Caching.ClearCache();
    }
}