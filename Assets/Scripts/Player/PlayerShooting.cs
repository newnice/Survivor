using UnityEngine;
using System.Text;

namespace Nightmare {
    public class PlayerShooting : PausableObject {
        public int damagePerShot = 20;
        public float timeBetweenBullets = 0.15f;
        public float range = 100f;
        public GameObject grenade;
        public float grenadeSpeed = 200f;
        public float grenadeFireDelay = 0.5f;

        float timer;
        Ray shootRay;
        RaycastHit shootHit;
        int shootableMask;
        ParticleSystem gunParticles;
        LineRenderer gunLine;
        AudioSource gunAudio;
        Light gunLight;
        public Light faceLight;
        float effectsDisplayTime = 0.2f;
        int grenadeStock = 0;

        void Awake() {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask("Shootable", "Enemy");

            // Set up the references.
            gunParticles = GetComponent<ParticleSystem>();
            gunLine = GetComponent<LineRenderer>();
            gunAudio = GetComponent<AudioSource>();
            gunLight = GetComponent<Light>();
            //faceLight = GetComponentInChildren<Light> ();

            AdjustGrenadeStock(0);
        }

        protected override void OnEnable() {
            base.OnEnable();
            EventManager.StartListening(NightmareEvent.CollectGrenade, o=>CollectGrenade());
        }

        protected override void OnDisable() {
            base.OnDisable();
            EventManager.StopListening(NightmareEvent.CollectGrenade, o=>CollectGrenade());
        }

        void Update() {
            if (IsPausedGame) return;
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            if (timer >= timeBetweenBullets && Time.timeScale != 0) {
                // If the Fire1 button is being press and it's time to fire...
                if (Input.GetButton("Fire2") && grenadeStock > 0) {
                    // ... shoot a grenade.
                    ShootGrenade();
                }

                // If the Fire1 button is being press and it's time to fire...
                else if (Input.GetButton("Fire1")) {
                    // ... shoot the gun.
                    Shoot();
                }
            }

#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if (timer >= timeBetweenBullets * effectsDisplayTime) {
                // ... disable the effects.
                DisableEffects();
            }
        }


        public void DisableEffects() {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
            faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot() {
            // Reset the timer.
            timer = 0f;

            // Play the gun shot audioclip.
            gunAudio.Play();

            // Enable the lights.
            gunLight.enabled = true;
            faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = transform.position;
            shootRay.direction = transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask)) {
                //DebugShot(shootHit);
                Debug.DrawRay(transform.position, range * transform.forward, Color.red, 1f);

                // Try and find an EnemyHealth script on the gameobject hit.
                EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                // If the EnemyHealth component exist...
                if (enemyHealth != null) {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition(1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }

        private void DebugShot(RaycastHit rayHit) {
            var sb = new StringBuilder();
            sb.Append("Hit: ");
            sb.Append(rayHit.collider.name);

            sb.Append("\nDistance: ");
            sb.Append(rayHit.distance);

            sb.Append("\nCollider: ");
            sb.Append(rayHit.collider);

            sb.Append("\nTrigger: ");
            sb.Append(rayHit.collider.isTrigger);

            sb.Append("\nPoint: ");
            sb.Append(rayHit.point);

            sb.Append("\nRigidbody: ");
            sb.Append(rayHit.rigidbody);

            Debug.Log(sb.ToString());
        }

        private void ChangeGunLine(float midPoint) {
            AnimationCurve curve = new AnimationCurve();

            curve.AddKey(0f, 0f);
            curve.AddKey(midPoint, 0.5f);
            curve.AddKey(1f, 1f);

            gunLine.widthCurve = curve;
        }

        private void CollectGrenade() {
            AdjustGrenadeStock(1);
        }

        private void AdjustGrenadeStock(int change) {
            grenadeStock += change;
        }

        void ShootGrenade() {
            AdjustGrenadeStock(-1);
            timer = timeBetweenBullets - grenadeFireDelay;
            PoolManager.Pull("Grenade", transform.position, Quaternion.identity);
            EventManager.TriggerEvent(NightmareEvent.ShootGrenade, grenadeSpeed * transform.forward);
        }
    }
}