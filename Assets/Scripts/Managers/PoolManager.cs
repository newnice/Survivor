using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Object;

public class PoolManager<T> : MonoBehaviour  where T:Pool{
    [SerializeField] private List<T> pools = null;
    private Dictionary<string, Pool> cache;

    protected Dictionary<string, Pool> Cache => cache;
    protected List<T> Pools => pools;
   
    protected virtual void Start() {
        if (pools == null) return;
        cache = new Dictionary<string, Pool>(pools.Count);
        InitPoolCache();
    }

    protected virtual void InitPoolCache() {
        foreach (var tempPool in pools) {
            cache[tempPool.key] = (T) new Pool(tempPool.key, tempPool.poolObjectPrefab, tempPool.size,
                tempPool.parentingGroup, tempPool.expandable);
        }
    } 

    /// <summary>
    /// Grabs the next item from the pool.
    /// </summary>
    /// <param name="key">Name of the pool to draw from.</param>
    /// <returns>Next free item.  Null if none available.</returns>
    public GameObject Pull(string key) {
        return cache[key].Pull();
    }

    public GameObject Pull(string key, Vector3 position, Quaternion rotation) {
        var clone = cache[key].Pull(false);
        clone.transform.position = position;
        clone.transform.rotation = rotation;
        clone.SetActive(true);
        return clone;
    }
}



[Serializable]
public class Pool {
    public string key;
    public int size;
    public Transform parentingGroup;
    public bool expandable;
    
    public GameObject poolObjectPrefab;
    
    private List<GameObject> _poolObjects;

    public Pool(string keyName, GameObject obj, int count, Transform parent = null, bool dynamicExpansion = false) {
        key = keyName;
        poolObjectPrefab = obj;
        size = count;
        expandable = dynamicExpansion;
        parentingGroup = parent;
        _poolObjects = new List<GameObject>();
        
        for (var i = 0; i < count; i++) {
            AddItem();
        }
    }

    public GameObject Pull(bool setActive = true) {
        // Is there one ready?
        foreach (var obj in _poolObjects) {
            if (obj.activeInHierarchy) continue;

            obj.SetActive(setActive);
            return obj;
        }

        // Can one be added?
        if (expandable) {
            return AddItem(true);
        }

        Debug.LogWarning("No available item from pool with key: " + key);
        return null;
    }

    private GameObject AddItem(bool keepActive = false) {
        var index = _poolObjects.Count;
        _poolObjects.Add(Instantiate(poolObjectPrefab));
        _poolObjects[index].name = poolObjectPrefab.name + "_" + index.ToString().PadLeft(4, '0');
        _poolObjects[index].SetActive(keepActive);
        if (parentingGroup != null) {
            _poolObjects[index].transform.parent = parentingGroup;
        }

        return _poolObjects[index];
    }
    
  
}