using UnityEngine;

namespace Nightmare {
    public class CameraFollow : MonoBehaviour {
        [SerializeField] private Transform target = null; // The position that that camera will be following.
        [SerializeField] private float smoothing = 5f; // The speed with which the camera will be following.
        private Vector3 _offset; // The initial offset from the target.

        void Start() {
            // Calculate the initial offset.
            _offset = transform.position - target.position;
        }


        void FixedUpdate() {
            // Create a postion the camera is aiming for based on the offset from the target.
            var targetCamPos = target.position + _offset;

            // Smoothly interpolate between the camera's current position and it's target position.
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}