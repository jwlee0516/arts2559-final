using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NetworkFPSPlayerController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private GameObject bodyVisual;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float flySpeed = 8f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 0.18f;
    [SerializeField] private float lookSmoothTime = 0.03f;
    [SerializeField] private float maxLookAngle = 85f;

    private CharacterController characterController;
    private PlayerControl controls;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 currentLookDelta;
    private Vector2 lookVelocity;

    private bool jumpQueued;
    private bool sprintHeld;
    private bool flyUpHeld;
    private bool flyDownHeld;
    private bool flyMode;

    private float verticalVelocity;
    private float pitch;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        controls = new PlayerControl();
    }

    public override void OnNetworkSpawn()
    {
        bool isLocalPlayer = IsOwner;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(isLocalPlayer);

        if (audioListener != null)
            audioListener.enabled = isLocalPlayer;

        if (bodyVisual != null)
            bodyVisual.SetActive(!isLocalPlayer);

        if (!isLocalPlayer)
        {
            enabled = false;
            return;
        }

        controls.Enable();
        SubscribeInput();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            UnsubscribeInput();
            controls.Disable();
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleLook();
        HandleMovement();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleLook()
    {
        Vector2 targetLookDelta = lookInput * lookSensitivity;

        currentLookDelta = Vector2.SmoothDamp(
            currentLookDelta,
            targetLookDelta,
            ref lookVelocity,
            lookSmoothTime
        );

        pitch -= currentLookDelta.y;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * currentLookDelta.x);
    }

    private void HandleMovement()
    {
        Vector3 horizontalMove =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        if (horizontalMove.sqrMagnitude > 1f)
            horizontalMove.Normalize();

        if (flyMode)
        {
            float verticalFly = 0f;

            if (flyUpHeld) verticalFly += 1f;
            if (flyDownHeld) verticalFly -= 1f;

            float currentFlySpeed = sprintHeld ? sprintSpeed : flySpeed;
            Vector3 flyMove = (horizontalMove + Vector3.up * verticalFly) * currentFlySpeed;

            characterController.Move(flyMove * Time.deltaTime);
            return;
        }

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (jumpQueued && characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        jumpQueued = false;
        verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed = sprintHeld ? sprintSpeed : walkSpeed;

        Vector3 finalMove = horizontalMove * currentSpeed;
        finalMove.y = verticalVelocity;

        characterController.Move(finalMove * Time.deltaTime);
    }

    private void SubscribeInput()
    {
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;

        controls.Player.Look.performed += OnLook;
        controls.Player.Look.canceled += OnLook;

        controls.Player.Jump.performed += OnJump;
        
        controls.Player.ToggleFly.performed += OnToggleFly;

        controls.Player.FlyUp.performed += OnFlyUpPerformed;
        controls.Player.FlyUp.canceled += OnFlyUpCanceled;

        controls.Player.FlyDown.performed += OnFlyDownPerformed;
        controls.Player.FlyDown.canceled += OnFlyDownCanceled;
    }

    private void UnsubscribeInput()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;

        controls.Player.Look.performed -= OnLook;
        controls.Player.Look.canceled -= OnLook;

        controls.Player.Jump.performed -= OnJump;
        controls.Player.ToggleFly.performed -= OnToggleFly;

        controls.Player.FlyUp.performed -= OnFlyUpPerformed;
        controls.Player.FlyUp.canceled -= OnFlyUpCanceled;

        controls.Player.FlyDown.performed -= OnFlyDownPerformed;
        controls.Player.FlyDown.canceled -= OnFlyDownCanceled;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpQueued = true;
    }
    
    private void OnToggleFly(InputAction.CallbackContext context)
    {
        flyMode = !flyMode;

        if (flyMode)
            verticalVelocity = 0f;
    }

    private void OnFlyUpPerformed(InputAction.CallbackContext context)
    {
        flyUpHeld = true;
    }

    private void OnFlyUpCanceled(InputAction.CallbackContext context)
    {
        flyUpHeld = false;
    }

    private void OnFlyDownPerformed(InputAction.CallbackContext context)
    {
        flyDownHeld = true;
    }

    private void OnFlyDownCanceled(InputAction.CallbackContext context)
    {
        flyDownHeld = false;
    }
}