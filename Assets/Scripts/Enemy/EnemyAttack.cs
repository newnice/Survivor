using UnityEngine;
using System.Collections;

namespace Nightmare {
    public class EnemyAttack : PausableObject {
        [SerializeField] private float timeBetweenAttacks = 0.5f;
        [SerializeField] private int attackDamage = 10;

        private Animator _anim;
        private GameObject _player;
        private PlayerHealth _playerHealth;
        private EnemyHealth _enemyHealth;
        private bool _playerInRange;
        private float _timer;

        void Awake() {
            // Setting up the references.
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerHealth = _player.GetComponent<PlayerHealth>();
            _enemyHealth = GetComponent<EnemyHealth>();
            _anim = GetComponent<Animator>();
        }

        void OnTriggerEnter(Collider other) {
            // If the entering collider is the player... the player is in range
            if (other.gameObject == _player) {
                _playerInRange = true;
            }
        }

        void OnTriggerExit(Collider other) {
            // If the exiting collider is the player... // ... the player is no longer in range.
            if (other.gameObject == _player) {
                _playerInRange = false;
            }
        }

        void Update() {
            if (IsPausedGame) return;
            // Add the time since Update was last called to the timer.
            _timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if (_timer >= timeBetweenAttacks && _playerInRange && _enemyHealth.CurrentHealth() > 0) {
                // ... attack.
                Attack();
            }

            // If the player has zero or less health...
            if (_playerHealth.currentHealth <= 0) {
                // ... tell the animator the player is dead.
                _anim.SetTrigger(AnimationConstants.PlayerDeadTrigger);
            }
        }

        private void Attack() {
            // Reset the timer.
            _timer = 0f;

            // If the player has health to lose... // ... damage the player.
            if (_playerHealth.currentHealth > 0) {
                _playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}