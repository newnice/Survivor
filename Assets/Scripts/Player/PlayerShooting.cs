using UnityEngine;

namespace Nightmare {
    public class PlayerShooting : PausableObject {
        private const float EffectsDisplayTime = 0.2f;

        public int damagePerShot = 20;
        public float timeBetweenBullets = 0.15f;
        public float range = 100f;
        public float grenadeFireDelay = 0.5f;
        public Light faceLight;

        private float _timer;
        private Ray _shootRay;
        private RaycastHit _shootHit;
        private int _shootableMask;
        private ParticleSystem _gunParticles;
        private LineRenderer _gunLine;
        private AudioSource _gunAudio;
        private Light _gunLight;


        private AdsManager _adsManager;
        private GrenadeManager _grenadeManager;

        void Awake() {
            // Create a layer mask for the Shootable layer.
            _shootableMask = LayerMask.GetMask("Shootable", "Enemy");

            // Set up the references.
            _gunParticles = GetComponent<ParticleSystem>();
            _gunLine = GetComponent<LineRenderer>();
            _gunAudio = GetComponent<AudioSource>();
            _gunLight = GetComponent<Light>();
            _adsManager = FindObjectOfType<AdsManager>();
            _grenadeManager = FindObjectOfType<GrenadeManager>();
        }


        void Update() {
            if (IsPausedGame) return;
            // Add the time since Update was last called to the timer.
            _timer += Time.deltaTime;

#if !MOBILE_INPUT
            if (_timer >= timeBetweenBullets && Time.timeScale != 0) {
                // If the Fire1 button is being press and it's time to fire...
                if (Input.GetButton("Fire2")) {
                    if (_grenadeManager.HasGrenades())
                        ShootGrenade();
                    else {
                        _adsManager.OfferAdsForGrenades();
                    }
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
            if (_timer >= timeBetweenBullets * EffectsDisplayTime) {
                // ... disable the effects.
                DisableEffects();
            }
        }


        public void DisableEffects() {
            // Disable the line renderer and the light.
            _gunLine.enabled = false;
            faceLight.enabled = false;
            _gunLight.enabled = false;
        }


        void Shoot() {
            // Reset the timer.
            _timer = 0f;

            // Play the gun shot audioclip.
            _gunAudio.Play();

            // Enable the lights.
            _gunLight.enabled = true;
            faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            _gunParticles.Stop();
            _gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            _gunLine.enabled = true;
            _gunLine.SetPosition(0, transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            _shootRay.origin = transform.position;
            _shootRay.direction = transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(_shootRay, out _shootHit, range, _shootableMask)) {
                //DebugShot(shootHit);
                Debug.DrawRay(transform.position, range * transform.forward, Color.red, 1f);

                // Try and find an EnemyHealth script on the gameobject hit.
                EnemyHealth enemyHealth = _shootHit.collider.GetComponent<EnemyHealth>();

                // If the EnemyHealth component exist...
                if (enemyHealth != null) {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage(damagePerShot);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                _gunLine.SetPosition(1, _shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                _gunLine.SetPosition(1, _shootRay.origin + _shootRay.direction * range);
            }
        }


        private void ChangeGunLine(float midPoint) {
            AnimationCurve curve = new AnimationCurve();

            curve.AddKey(0f, 0f);
            curve.AddKey(midPoint, 0.5f);
            curve.AddKey(1f, 1f);

            _gunLine.widthCurve = curve;
        }


        void ShootGrenade() {
            _timer = timeBetweenBullets - grenadeFireDelay;
            EventManager.TriggerEvent(NightmareEvent.ShootGrenade, transform);
        }
    }
}