using UnityEngine;

public class AssetBundleLoader : MonoBehaviour {
    public static GameObject LoadAsset(string assetBundleName, string assetName) {
        //TODO: remove  relative to platform path name
        var myLoadedAssetBundle = AssetBundle.LoadFromFile($"{Application.dataPath}/../AssetBundles/StandaloneWindows/{assetBundleName}");
  
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