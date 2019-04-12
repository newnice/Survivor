using UnityEngine;
using UnityEngine.AI;

namespace Nightmare {
    public class EnemyMovement : PausableObject {
        [SerializeField] private float visionRange = 10f;
        [SerializeField] private float hearingRange = 20f;
        [SerializeField] private float wanderDistance = 10f;
        [SerializeField] private Vector2 idleTimeRange = Vector2.zero;
        [SerializeField] [Range(0f, 1f)] private float psychicLevels = 0.2f;
        [SerializeField] private float timer = 0f;

        private float _currentVision;
        private Transform _player;
        private PlayerHealth _playerHealth;
        private EnemyHealth _enemyHealth;
        private NavMeshAgent _nav;
        private Animator _anim;

        void Awake() {
            _player = GameObject.FindGameObjectWithTag(TagNames.Player).transform;
            _playerHealth = _player.GetComponent<PlayerHealth>();
            _enemyHealth = GetComponent<EnemyHealth>();
            _nav = GetComponent<NavMeshAgent>();
            _nav.avoidancePriority += Random.Range(-5, 6);
            _anim = GetComponent<Animator>();
        }

        protected override void OnEnable() {
            base.OnEnable();
            _nav.enabled = true;
            ClearPath();
            ScaleVision(1f);
            IsPsychic();
            timer = 0f;
            EventManager.StartListening(NightmareEvent.GrenadeExploded, grenadePos=>HearPoint((Vector3) grenadePos));
        }

        protected override void OnDisable() {
            base.OnDisable();
            EventManager.StopListening(NightmareEvent.GrenadeExploded, grenadePos=>HearPoint((Vector3) grenadePos));
        }

        protected override void OnPause(bool isPaused) {
            base.OnPause(isPaused);
            _nav.enabled = !isPaused;
            _anim.enabled = !isPaused;
        }

        void ClearPath() {
            if (_nav.hasPath)
                _nav.ResetPath();
        }

        void Update() {
            if (IsPausedGame) return;
            // If both the enemy and the player have health left...
            if (_enemyHealth.CurrentHealth() > 0 && _playerHealth.currentHealth > 0) {
                LookForPlayer();
                WanderOrIdle();
            }
            else {
                _nav.enabled = false;
            }
        }

        void OnDestroy() {
            _nav.enabled = false;
        }

        private void LookForPlayer() {
            TestSense(_player.position, _currentVision);
        }

        private void HearPoint(Vector3 position) {
            TestSense(position, hearingRange);
        }

        private void TestSense(Vector3 position, float senseRange) {
            if (Vector3.Distance(transform.position, position) <= senseRange) {
                GoToPosition(position);
            }
        }

        public void GoToPlayer() {
            GoToPosition(_player.position);
        }

        private void GoToPosition(Vector3 position) {
            if (IsPausedGame) return;
            timer = -1f;
            if (!_enemyHealth.IsDead()) {
                SetDestination(position);
            }
        }

        private void SetDestination(Vector3 position) {
            if (_nav.isOnNavMesh) {
                _nav.SetDestination(position);
            }
        }

        private void WanderOrIdle() {
            if (_nav.hasPath) return;
            if (timer <= 0f) {
                SetDestination(GetRandomPoint(wanderDistance, 5));
                if (_nav.pathStatus == NavMeshPathStatus.PathInvalid) {
                    ClearPath();
                }

                timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
            }
            else {
                timer -= Time.deltaTime;
            }
        }

        private void IsPsychic() {
            if (Random.Range(0f, 1f) < psychicLevels) {
                GoToPlayer();
            }
        }

        private Vector3 GetRandomPoint(float distance, int layermask) {
            var randomPoint = Random.insideUnitSphere * distance + transform.position;

            NavMesh.SamplePosition(randomPoint, out var navHit, distance, layermask);

            return navHit.position;
        }

        public void ScaleVision(float scale) {
            _currentVision = visionRange * scale;
        }

        private int GetCurrentNavArea() {
            NavMeshHit navHit;
            _nav.SamplePathPosition(-1, 0.0f, out navHit);

            return navHit.mask;
        }
    }
}