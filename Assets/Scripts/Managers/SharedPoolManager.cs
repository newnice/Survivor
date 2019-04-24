using UnityEngine;

namespace Nightmare {
    public class SharedPoolManager : PoolManager<Pool> {
        private static SharedPoolManager _sharedManger;

        public static SharedPoolManager Instance {
            get {
                if (_sharedManger == null) {
                    _sharedManger = FindObjectOfType(typeof(SharedPoolManager)) as SharedPoolManager;

                    if (_sharedManger == null) {
                        Debug.LogError("There needs to be one active PoolManger script on a GameObject in your scene.");
                    }
                }

                return _sharedManger;
            }
        }
    }
}