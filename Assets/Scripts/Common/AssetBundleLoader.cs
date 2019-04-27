using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour {
    private static readonly Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();

    public static GameObject LoadAsset(string assetBundleName, string assetName) {
        if (!_loadedBundles.TryGetValue(assetBundleName, out var myLoadedAssetBundle)) {
            var path = $"{Application.streamingAssetsPath}/{assetBundleName}";
            myLoadedAssetBundle = AssetBundle.LoadFromFile(path);

            if (myLoadedAssetBundle == null) {
                Debug.Log($"Failed to load AssetBundle {assetBundleName}!");
                return null;
            }

            _loadedBundles.Add(assetBundleName, myLoadedAssetBundle);
        }


        if (assetName != null) {
            var assetObject = myLoadedAssetBundle.LoadAsset<GameObject>(assetName);
            return assetObject;
        }

        myLoadedAssetBundle.LoadAllAssets();
        return null;
    }

    public static void UnloadBundle(string bundleName) {
        if (_loadedBundles.TryGetValue(bundleName, out var bundle)) {
            bundle.Unload(false);
            _loadedBundles.Remove(bundleName);
        }
    }

    public static void UnloadAllBundles() {
        foreach (var bundle in _loadedBundles.Values) {
            bundle.Unload(false);
        }
        _loadedBundles.Clear();
    }
    
    

    public static void ClearLoadCache() {
        Caching.ClearCache();
    }
}