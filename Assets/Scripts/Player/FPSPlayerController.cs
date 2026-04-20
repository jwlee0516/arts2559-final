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
    [SerializeField] private float flySpeed = 8f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    [Header("Mouse Look")]
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float maxLookX = 85f;

    private CharacterController controller;
    private PlayerControl controls;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool flyUpHeld;
    private bool flyDownHeld;

    private float verticalVelocity;
    private float cameraPitch;

    private bool flyMode = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
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

        controls.Player.FlyUp.performed -= OnFlyUpPerformed;
        controls.Player.FlyUp.canceled -= OnFlyUpCanceled;

        controls.Player.FlyDown.performed -= OnFlyDownPerformed;
        controls.Player.FlyDown.canceled -= OnFlyDownCanceled;

        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookX, maxLookX);

        cameraPivot.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (flyMode)
        {
            float verticalFly = 0f;
            if (flyUpHeld) verticalFly += 1f;
            if (flyDownHeld) verticalFly -= 1f;

            Vector3 flyMove = (move + Vector3.up * verticalFly) * flySpeed;
            controller.Move(flyMove * Time.deltaTime);
            return;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (jumpPressed && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        jumpPressed = false;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * walkSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    public void ToggleFlyMode()
    {
        flyMode = !flyMode;
        if (flyMode)
        {
            verticalVelocity = 0f;
        }
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
        jumpPressed = true;
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