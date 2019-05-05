using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

namespace Nightmare {
    public class PlayerMovement : PausableObject {
        [SerializeField] private float speed = 6f; // The speed that the player will move at.


        private Vector3 _movement; // The vector to store the direction of the player's movement.
        private Animator _anim; // Reference to the animator component.
        private Rigidbody _playerRigidbody; // Reference to the player's rigidbody.

        private Vector3 _initialPosition;
        private UnityAction<object> _onGameOverAction;
#if !MOBILE_INPUT
        private int _floorMask; // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        private const float CamRayLength = 100f; // The length of the ray from the camera into the scene.
#endif

        void Awake() {
#if !MOBILE_INPUT
            // Create a layer mask for the floor layer.
            _floorMask = LayerMask.GetMask("Floor");
#endif

            // Set up references.
            _anim = GetComponent<Animator>();
            _playerRigidbody = GetComponent<Rigidbody>();
            _initialPosition = transform.position;
            _onGameOverAction = o => ResetPosition();
        }

        protected override void OnEnable() {
            base.OnEnable();
            EventManager.StartListening(NightmareEvent.GameOver, _onGameOverAction);
        }

        protected override void OnDisable() {
            base.OnDisable();
            EventManager.StopListening(NightmareEvent.GameOver, _onGameOverAction);
        }

        private void ResetPosition() {
            transform.position = _initialPosition;
        }

        protected override void OnPause(bool isPaused) {
            base.OnPause(isPaused);
            _anim.enabled = !isPaused;
        }

        void FixedUpdate() {
            if (IsPausedGame) return;
            // Store the input axes.
            var h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            var v = CrossPlatformInputManager.GetAxisRaw("Vertical");

            // Move the player around the scene.
            Move(h, v);

            // Turn the player to face the mouse cursor.
            Turning();

            // Animate the player.
            Animating(h, v);
        }


        private void Move(float h, float v) {
            // Set the movement vector based on the axis input.
            _movement.Set(h, 0f, v);

            // Normalise the movement vector and make it proportional to the speed per second.
            _movement = _movement.normalized * speed * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            _playerRigidbody.MovePosition(transform.position + _movement);
        }


        private void Turning() {
#if !MOBILE_INPUT
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Perform the raycast and if it hits something on the floor layer...
            if (Physics.Raycast(camRay, out var floorHit, CamRayLength, _floorMask)) {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                var playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                var newRotatation = Quaternion.LookRotation(playerToMouse);


                // Set the player's rotation to this new rotation.
                _playerRigidbody.MoveRotation(newRotatation);
            }
#else
            var turnDir =
                new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X"), 0f,
                    CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir == Vector3.zero) return;

            var playerToMouse =  turnDir;
            playerToMouse.y = 0f;

            var newRotatation = Quaternion.LookRotation(playerToMouse);

            _playerRigidbody.MoveRotation(newRotatation);
#endif
        }


        void Animating(float h, float v) {
            // Create a boolean that is true if either of the input axes is non-zero.
            var walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            _anim.SetBool("IsWalking", walking);
        }
    }
}