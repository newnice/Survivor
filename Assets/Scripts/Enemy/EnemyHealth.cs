using UnityEngine;

namespace Nightmare {
    public class EnemyHealth : MonoBehaviour {
        public int startingHealth = 100;
        public float sinkSpeed = 2.5f;
        public int scoreValue = 10;
        public AudioClip deathClip;

        private int _currentHealth;
        private Animator _anim;
        private AudioSource _enemyAudio;
        private ParticleSystem _hitParticles;
        private CapsuleCollider _capsuleCollider;
        private EnemyMovement _enemyMovement;

        void Awake() {
            _anim = GetComponent<Animator>();
            _enemyAudio = GetComponent<AudioSource>();
            _hitParticles = GetComponentInChildren<ParticleSystem>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _enemyMovement = GetComponent<EnemyMovement>();
        }

        void OnEnable() {
            _currentHealth = startingHealth;
            SetKinematics(false);
        }

        private void SetKinematics(bool isKinematic) {
            _capsuleCollider.isTrigger = isKinematic;
            _capsuleCollider.attachedRigidbody.isKinematic = isKinematic;
        }

        void Update() {
            if (!IsDead()) return;
            
            transform.Translate(sinkSpeed * Time.deltaTime * -Vector3.up);
            if (transform.position.y < -10f) {
                Destroy(gameObject);
            }
        }

        public bool IsDead() {
            return _currentHealth <= 0f;
        }

        public void TakeDamage(int amount, Vector3 hitPoint) {
            ApplyDamage(amount);

            _hitParticles.transform.position = hitPoint;
            _hitParticles.Play();
        }

        public void TakeDamage(int amount) {
            ApplyDamage(amount);
        }

        private void ApplyDamage(int amount) {
            if (!IsDead()) {
                _enemyAudio.Play();
                _currentHealth -= amount;

                if (IsDead()) {
                    Death();
                }
                else {
                    _enemyMovement.GoToPlayer();
                }
            }
        }

        void Death() {
            _anim.SetTrigger(AnimationConstants.DeadTrigger);

            _enemyAudio.clip = deathClip;
            _enemyAudio.Play();
        }

        /**
         * Function for animation
         */
        private void StartSinking() {
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            SetKinematics(true);

            EventManager.TriggerEvent(NightmareEvent.EnemyKilled, scoreValue);
        }

        public int CurrentHealth() {
            return _currentHealth;
        }
        
        
    }
}