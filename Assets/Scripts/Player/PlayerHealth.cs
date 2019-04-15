using UnityEngine;
using UnityEngine.UI;

namespace Nightmare {
    public class PlayerHealth : MonoBehaviour {
        public int startingHealth = 100;
        public int currentHealth;
        public Slider healthSlider;
        public Image damageImage;
        public AudioClip deathClip;
        public float flashSpeed = 5f;
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
        public bool godMode;

        Animator anim;
        AudioSource playerAudio;
        PlayerMovement playerMovement;
        PlayerShooting playerShooting;
        bool isDead;
        bool damaged;

        void Awake() {
            // Setting up the references.
            anim = GetComponent<Animator>();
            playerAudio = GetComponent<AudioSource>();
            playerMovement = GetComponent<PlayerMovement>();
            playerShooting = GetComponentInChildren<PlayerShooting>();

            ResetPlayer();
        }

        protected virtual  void OnEnable() {
            EventManager.StartListening(NightmareEvent.LevelCompleted, score=>CelebrateLevelComplete((int) score));
        }

        private void OnDisable() {
            EventManager.StopListening(NightmareEvent.LevelCompleted, score=>CelebrateLevelComplete((int) score));
        }

        private void CelebrateLevelComplete(int currentScore) {
            anim.SetInteger(AnimationConstants.CurrentScoreAttribute, currentScore);
            anim.SetTrigger(AnimationConstants.LevelCompleteTrigger);
        }
        
        private void ResetPlayer() {
            anim.SetBool(AnimationConstants.IsDeadAttribute, false);
            isDead = false;
            // Set the initial health of the player.
            currentHealth = startingHealth;
            healthSlider.value = currentHealth;
            
            playerMovement.enabled = true;
            playerShooting.enabled = true;


        }


        void Update() {
            // If the player has just been damaged...
            if (damaged) {
                // ... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            // Otherwise...
            else {
                // ... transition the colour back to clear.
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Reset the damaged flag.
            damaged = false;
        }


        public void TakeDamage(int amount) {
            if (godMode)
                return;

            // Set the damaged flag so the screen will flash.
            damaged = true;

            // Reduce the current health by the damage amount.
            currentHealth -= amount;

            // Set the health bar's value to the current health.
            healthSlider.value = currentHealth;

            // Play the hurt sound effect.
            playerAudio.Play();

            // If the player has lost all it's health and the death flag hasn't been set yet...
            if (currentHealth <= 0 && !isDead) {
                // ... it should die.
                Death();
            }
        }


        void Death() {
            // Set the death flag so this function won't be called again.
            isDead = true;

            // Turn off any remaining shooting effects.
            playerShooting.DisableEffects();

            // Tell the animator that the player is dead.
            anim.SetBool(AnimationConstants.IsDeadAttribute, true);

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play();

            // Turn off the movement and shooting scripts.
            playerMovement.enabled = false;
            playerShooting.enabled = false;
        }

        /**
         * Function for animation event after state Death completed
         */
        private void OnPlayerDeath() {
            EventManager.TriggerEvent(NightmareEvent.GameOver);
            ResetPlayer();
        }
    }
}