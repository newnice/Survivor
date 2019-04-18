using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nightmare {
    [Serializable]
    public class SpawnConfiguration {
        public GameObject enemyPrefab;
        public float spawnTime;
        internal float leftTime;
    }

    public class EnemyManager : PausableObject {
        [SerializeField] private int maxEnemyCount = 100;
        [SerializeField] [Range(0f, 1f)] private float broadcastProbability = 0.1f;
        [SerializeField] private List<SpawnConfiguration> spawnConfig = null;
        [SerializeField] private List<Transform> spawnPoints = null;

        private bool _isGameStopped;
        private CullingGroup _cullingGroup;
        private IDictionary<Transform, bool> _notVisibleSpawnPoints = new Dictionary<Transform, bool>();
        public int _enemyCount;
        private Transform _player;
        private PoolManager _poolManager;

        protected void Start() {
            _player = GameObject.FindGameObjectWithTag(TagNames.Player).transform;
            _poolManager = GetComponent<PoolManager>();

            foreach (var spawnConfiguration in spawnConfig) {
                var sc = spawnConfiguration;
                sc.leftTime = sc.spawnTime;
            }

            spawnPoints.ForEach(sp => _notVisibleSpawnPoints.Add(sp, true));
            InitCullingGroupForSpawnPoints();
        }

        private void InitCullingGroupForSpawnPoints() {
            _cullingGroup = new CullingGroup {targetCamera = Camera.main};
            var bss = spawnPoints.Select(sp => new BoundingSphere(sp.position, 2f)).ToArray();
            _cullingGroup.SetBoundingSpheres(bss);
            _cullingGroup.SetBoundingSphereCount(bss.Length);
            _cullingGroup.onStateChanged += OnPointVisibilityChanged;
        }

        private void OnPointVisibilityChanged(CullingGroupEvent cullEvent) {
            if (cullEvent.hasBecomeVisible || cullEvent.hasBecomeInvisible)
                _notVisibleSpawnPoints[spawnPoints[cullEvent.index]] = cullEvent.hasBecomeInvisible;
        }

        protected override void OnEnable() {
            base.OnEnable();
            EventManager.StartListening(NightmareEvent.GameOver, o => StopSpawning(true));
            EventManager.StartListening(NightmareEvent.RestartGame, o => StopSpawning(false));
            EventManager.StartListening(NightmareEvent.EnemyKilled, o => UpdateEnemyCount(-1));
        }

        private void UpdateEnemyCount(int delta) {
            _enemyCount += delta;
        }

        protected override void OnDisable() {
            base.OnDisable();
            EventManager.StopListening(NightmareEvent.GameOver, o => StopSpawning(true));
            EventManager.StopListening(NightmareEvent.RestartGame, o => StopSpawning(false));
            EventManager.StopListening(NightmareEvent.EnemyKilled, o => UpdateEnemyCount(-1));
            _cullingGroup.Dispose();
        }

        private void StopSpawning(bool stop) {
            _isGameStopped = stop;
        }

        private void Update() {
            if (IsPausedGame || _isGameStopped) return;
            TrySpawn();
        }

        private void TrySendBroadcast() {
            if (Random.Range(0f, 1f) < broadcastProbability) return;
            
            EventManager.TriggerEvent(NightmareEvent.NoisePropagated, _player.position);
            Debug.Log("Send broadcast with player position");
        }

        private void TrySpawn() {
            var enemies = FindEnemiesToSpawn();
            enemies.ForEach(enemy => {
                if (_enemyCount < maxEnemyCount)
                    Spawn(enemy);
                else
                    TrySendBroadcast();
            });
        }

        private List<SpawnConfiguration> FindEnemiesToSpawn() {
            var toSpawn = new List<SpawnConfiguration>();
            foreach (var spawnConfiguration in spawnConfig) {
                var sc = spawnConfiguration;
                var leftTime = sc.leftTime - Time.deltaTime;
                if (leftTime < 0) {
                    sc.leftTime = sc.spawnTime;
                    toSpawn.Add(sc);
                }
                else
                    sc.leftTime = leftTime;
            }

            return toSpawn;
        }

        private void Spawn(SpawnConfiguration enemy) {
            var spawnPoint = SelectSpawnPoint();
            if (spawnPoint == null) {
                Debug.Log("Can't spawn because of all spawn points is visible by player...");
                return;
            }

            var enemyClone = _poolManager.Pull(enemy.enemyPrefab.name, spawnPoint.position, spawnPoint.rotation);
            enemyClone.transform.parent = transform;
            UpdateEnemyCount(1);
        }

        private Transform SelectSpawnPoint() {
            var notVisiblePoints = _notVisibleSpawnPoints.Where(p => p.Value).Select(pair => pair.Key).ToList();
            return notVisiblePoints.Count == 0 ? null : notVisiblePoints[Random.Range(0, notVisiblePoints.Count)];
        }
    }
}