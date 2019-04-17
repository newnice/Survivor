using UnityEngine;

namespace Nightmare {
    
    public class EnemyManager : PausableObject {
        private PlayerHealth _playerHealth;
        public GameObject enemy;
        public float spawnTime = 3f;
        public Transform[] spawnPoints;

        private float timer;

        void Start() {
            timer = spawnTime;
        }

        protected override void OnEnable() {
            base.OnEnable();
            _playerHealth = FindObjectOfType<PlayerHealth>();
        }

        void Update() {
            if(IsPausedGame) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                Spawn();
                timer = spawnTime;
            }
        }

        void Spawn() {
            // If the player has no health left...
            if (_playerHealth.currentHealth <= 0f) {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.

            var enemyClone = Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
            enemyClone.transform.parent = transform;
        }
        
        
    }
}