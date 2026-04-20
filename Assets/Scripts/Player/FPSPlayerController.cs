using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float flySpeed = 8f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 0.18f;
    [SerializeField] private float lookSmoothTime = 0.03f;
    [SerializeField] private bool useLookSmoothing = true;
    [SerializeField] private float maxLookAngle = 85f;

    private Vector2 currentLookDelta;
    private Vector2 currentLookVelocity;
    

    private CharacterController characterController;
    private PlayerControl controls;

    private Vector2 moveInput;
    private Vector2 lookInput;

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

    private void OnEnable()
    {
        controls.Enable();

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

    private void OnDisable()
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

        controls.Disable();
    }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }

    private void HandleLook()
    {
        Vector2 targetLookDelta = lookInput * lookSensitivity;

        if (useLookSmoothing)
        {
            currentLookDelta = Vector2.SmoothDamp(
                currentLookDelta,
                targetLookDelta,
                ref currentLookVelocity,
                lookSmoothTime
            );
        }
        else
        {
            currentLookDelta = targetLookDelta;
        }

        float mouseX = currentLookDelta.x;
        float mouseY = currentLookDelta.y;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
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

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        {
            verticalVelocity = 0f;
        }
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