using System.Collections;
using UnityEngine;

namespace Nightmare {
    public class Grenade : PausableObject {
        public float explosiveForce = 500f;
        public int explosiveDamage = 50;
        public float explosiveRadius = 2f;
        public float timeOut = 3f;

        private bool isPickup;
        private Rigidbody rb;
        private ParticleSystem ps;
        private MeshRenderer mr;
        private TrailRenderer tr;
        private float timer = 0f;
        private float destroyWait;

        void Awake() {
            rb = GetComponent<Rigidbody>();
            mr = GetComponent<MeshRenderer>();
            tr = GetComponentInChildren<TrailRenderer>();
            ps = GetComponentInChildren<ParticleSystem>();

            ParticleSystem.MainModule pMain = ps.main;
            destroyWait = Mathf.Max(pMain.startLifetime.constantMin, pMain.startLifetime.constantMax);
        }

        protected override void OnEnable() {
            base.OnEnable();
            timer = 0f;
            mr.enabled = true;
            tr.enabled = false;
            ps.Stop();
            isPickup = true;
            EventManager.StartListening(NightmareEvent.ShootGrenade, Shoot);
        }

        protected override void OnPause(bool isPaused) { }

        void Update() {
            if (IsPausedGame) return;
            
            if (!(timer > 0f)) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                Explode();
            }
        }

        void OnTriggerEnter(Collider coll) {
            if (isPickup) {
                if (coll.CompareTag(TagNames.Player)) {
                    Disable();
                    EventManager.TriggerEvent(NightmareEvent.CollectGrenade);
                }
            }
            else {
                if (coll.CompareTag(TagNames.Enemy)) {
                    Explode();
                }
            }
        }

        private void Shoot(Vector3 force) {
            if (timer > 0f)
                return;

            isPickup = false;
            mr.enabled = true;
            tr.enabled = true;
            timer = timeOut;
            rb.AddForce(force);
        }

        private void Explode() {
            timer = -1;
            ps.Play();
            tr.enabled = false;
            mr.enabled = false;

            var colls = Physics.OverlapSphere(transform.position, explosiveRadius);
            foreach (var col in colls) {
                if (!col.CompareTag(TagNames.Enemy) || col.isTrigger) continue;
                var victim = col.GetComponent<EnemyHealth>();
                if (victim != null) {
                    victim.TakeDamage(explosiveDamage);
                }
            }

            StartCoroutine("TimedDisable");
        }

        private IEnumerator TimedDisable() {
            yield return new WaitForSeconds(destroyWait);
            Disable();
        }

        private void Disable() {
            timer = -1;
            isPickup = false;
            gameObject.SetActive(false);
            EventManager.StopListening(NightmareEvent.ShootGrenade, Shoot);
        }
    }
}