using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float gravity = -9.81f;

    [Header("시야")]
    public float lookSensitivity = 1f;
    public Transform cameraHolder;

    private CharacterController controller;
    private PlayerInteraction interaction;  //  연결

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;

    private float verticalRotation;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        interaction = GetComponent<PlayerInteraction>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) isSprinting = true;
        if (context.canceled) isSprinting = false;
    }

    //  F 키 입력 처리
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryInteract();
    }

    private void Update()
    {
        Move();
        Look();
    }

    private void Move()
    {
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void Look()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    // 상호작용 실행 (레이캐스트 이미 PlayerInteraction이 함)
    private void TryInteract()
    {
        if (interaction == null)
        {
            Debug.LogWarning("PlayerInteraction이 연결되지 않았습니다.");
            return;
        }

        IInteractable target = interaction.CurrentTarget;
        if (target != null)
        {
            target.Interact(interaction);
        }
        else
        {
            Debug.Log("대상이 없습니다.");
        }
    }

}
