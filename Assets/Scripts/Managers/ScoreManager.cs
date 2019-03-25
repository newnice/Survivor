using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Nightmare
{
    public class ScoreManager : MonoBehaviour
    {
        public static int score;

        Text sText;

        void Awake ()
        {
            sText = GetComponent <Text> ();

            score = 0;
        }


        void Update ()
        {
            sText.text = "Score: " + score;
        }
    }
}