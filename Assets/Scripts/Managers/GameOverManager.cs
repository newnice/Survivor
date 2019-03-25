using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Nightmare
{
    public class GameOverManager : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        Animator anim;

        private UnityEvent listener;

        void Awake ()
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            anim = GetComponent <Animator> ();
        }

        void ShowGameOver()
        {
            anim.SetBool("GameOver", true);
        }

        private void ResetLevel()
        {
            ScoreManager.score = 0;
            anim.SetBool("GameOver", false);
            playerHealth.ResetPlayer();
        }
    }
}