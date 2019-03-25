using System.Collections;
using UnityEngine;

namespace Nightmare {
    public class Grenade : MonoBehaviour {
        public float explosiveForce = 500f;
        public int explosiveDamage = 50;
        public float explosiveRadius = 2f;
        public float timeOut = 3f;

        bool isPickup;
        Rigidbody rb;
        ParticleSystem ps;
        MeshRenderer mr;
        TrailRenderer tr;
        float timer = 0f;
        float destroyWait;

        void Awake() {
            rb = GetComponent<Rigidbody>();
            mr = GetComponent<MeshRenderer>();
            tr = GetComponentInChildren<TrailRenderer>();
            ps = GetComponentInChildren<ParticleSystem>();

            ParticleSystem.MainModule pMain = ps.main;
            destroyWait = Mathf.Max(pMain.startLifetime.constantMin, pMain.startLifetime.constantMax);
        }

        void OnEnable() {
            timer = 0f;
            mr.enabled = true;
            tr.enabled = false;
            ps.Stop();
            isPickup = true;
        }

        void Update() {
            if (!(timer > 0f)) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                Explode();
            }
        }

        void OnTriggerEnter(Collider coll) {
            if (isPickup) {
                Disable();
                EventManager.TriggerEvent(NightmareEvent.CollectGrenade);
            }
            else {
                if (coll.CompareTag("Enemy")) {
                    Explode();
                }
            }
        }

        public void Shoot(Vector3 force) {
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
            for (int i = 0; i < colls.Length; i++) {
                if (!colls[i].CompareTag("Enemy") || colls[i].isTrigger) continue;
                var victim = colls[i].GetComponent<EnemyHealth>();
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
        }
    }
}