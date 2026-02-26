using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        // public Transform playerCamera;
        
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float jumpForce = 5f;

        [Header("Mouse Settings")]
        public float mouseSensitivity = 200f;
        public Vector2 pitchMinMax = new Vector2(-40, 85);

        [Header("Input")]
        public InputActionAsset inputActions;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
    
        private Rigidbody _rb;
        private Camera _camera;
        private float _verticalLookRotation;
        private bool _isGrounded;
    
        [Header("Debug")]
        private Vector3 _initPosition;
        private InputAction _resetActionD;
    
        private void OnEnable()
        {
            inputActions.FindActionMap("Player").Enable();
            inputActions.FindActionMap("Debug").Enable();
        }

        private void OnDisable()
        {
            inputActions.FindActionMap("Player").Disable();
            inputActions.FindActionMap("Debug").Disable();
        }

        private void Awake()
        {
            _camera = GetComponentInChildren<Camera> ();
            _rb = GetComponent<Rigidbody>();

            _moveAction = inputActions.FindAction("Move");
            _lookAction = inputActions.FindAction("Look");
            _jumpAction = inputActions.FindAction("Jump");
            
            _resetActionD = inputActions.FindAction("ResetPlayer");

        }

        private void Start()
        {
            _rb.freezeRotation = true;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            HandleMouseLook();
            if (_jumpAction.WasPressedThisFrame()) { HandleJump(); }
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMouseLook()
        {
            var lookAmt = _lookAction.ReadValue<Vector2>() * mouseSensitivity;
            
            // yaw (rotate transform)
            transform.Rotate(Vector3.up * lookAmt.x);

            // pitch (rotate camera)
            _verticalLookRotation -= lookAmt.y;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, pitchMinMax.x, pitchMinMax.y);

            _camera.transform.localRotation = Quaternion.Euler(_verticalLookRotation, 0f, 0f);
        }

        private void HandleMovement()
        {
            var moveAmt = _moveAction.ReadValue<Vector2>() * mouseSensitivity;

            Vector3 moveDirection = transform.right * moveAmt.x + transform.forward * moveAmt.y;
            Vector3 velocity = moveDirection * moveSpeed;

            Vector3 newVelocity = new Vector3(
                velocity.x,
                _rb.linearVelocity.y,
                velocity.z
            );

            _rb.linearVelocity = newVelocity;
        }

        private void HandleJump()
        {
            // Basic ground check
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

            if (_isGrounded)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
        }
    }
}