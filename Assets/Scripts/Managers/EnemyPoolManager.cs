using System;
using UnityEngine;

namespace Nightmare {
    public class EnemyPoolManager : PoolManager<EnemyPool> {
        [SerializeField] private string enemyCommonBundle="common";

        protected override void InitPoolCache() {
            AssetBundleLoader.LoadAsset(enemyCommonBundle, null);
            foreach (var tempPool in Pools) {
                var poolObject = AssetBundleLoader.LoadAsset(tempPool.enemyAssetName, tempPool.key);
                Cache[tempPool.key] = new Pool(tempPool.key,
                    poolObject != null ? poolObject : tempPool.poolObjectPrefab,
                    tempPool.size,
                    tempPool.parentingGroup, tempPool.expandable);
            }

            AssetBundleLoader.UnloadAllBundles();
            AssetBundleLoader.ClearLoadCache();
        }
    }

    [Serializable]
    public class EnemyPool : Pool {
        public string enemyAssetName;

        public EnemyPool(string keyName, GameObject obj, int count, Transform parent = null,
            bool dynamicExpansion = false) :
            base(keyName, obj, count, parent, dynamicExpansion) { }
    }

   /*TODO: remove from Inspector ability to set EnemyPool object prefab property
    [CustomPropertyDrawer(typeof(EnemyPool))]
    public class EnemyPoolEditor : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            EditorGUI.indentLevel++;
            
            var properties = new[] {"key", "enemyAssetName", "size", "parentingGroup", "expandable"};

            var counter = 1;
            foreach (var p in properties) {
                position.y += 30;
                EditorGUI.PropertyField(position, property.FindPropertyRelative(p), GUIContent.none);
                counter++;
            }
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }
    }*/
}