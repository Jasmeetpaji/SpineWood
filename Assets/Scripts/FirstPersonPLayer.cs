using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.5f;
    [Header("Gravity")]
    public float gravity = -20f;
    [Header("Camera")]
    public Transform playerCamera;
    [Header("Head Bob")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float sprintBobSpeed = 12f;
    public float sprintBobAmount = 0.08f;
    [Header("Camera Sway")]
    public float swayAmount = 2f;
    public float swaySpeed = 8f;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 cameraStartPosition;
    private float bobTimer = 0f;
    private bool isMoving = false;
    private bool isSprinting = false;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera != null)
        {
            cameraStartPosition = playerCamera.localPosition;
        }
        else
        {
            Debug.LogError(
                "FirstPersonPlayer: Player Camera is not assigned!"
            );
        }
    }
    void Update()
    {
        MovePlayer();
        HeadBob();
    }
    void MovePlayer()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1;
            if (Keyboard.current.sKey.isPressed)
                input.y -= 1;
            if (Keyboard.current.dKey.isPressed)
                input.x += 1;
            if (Keyboard.current.aKey.isPressed)
                input.x -= 1;
        }
        input = Vector2.ClampMagnitude(input, 1f);
        Vector3 move =
            transform.right * input.x +
            transform.forward * input.y;
        isMoving = input.magnitude > 0.1f;
        isSprinting =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed &&
            isMoving;
        float currentSpeed =
            isSprinting ? sprintSpeed : moveSpeed;
        controller.Move(
            move * currentSpeed * Time.deltaTime
        );
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded
        )
        {
            velocity.y =
                Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(
            velocity * Time.deltaTime
        );
    }
    void HeadBob()
    {
        if (playerCamera == null)
            return;
        if (isMoving && controller.isGrounded)
        {
            float bobSpeed =
                isSprinting
                    ? sprintBobSpeed
                    : walkBobSpeed;
            float bobAmount =
                isSprinting
                    ? sprintBobAmount
                    : walkBobAmount;
            bobTimer += Time.deltaTime * bobSpeed;
            float bobX =
                Mathf.Cos(bobTimer * 0.5f) *
                bobAmount;
            float bobY =
                Mathf.Sin(bobTimer) *
                bobAmount;
            Vector3 targetPosition =
                cameraStartPosition +
                new Vector3(
                    bobX,
                    bobY,
                    0f
                );
            playerCamera.localPosition =
                Vector3.Lerp(
                    playerCamera.localPosition,
                    targetPosition,
                    Time.deltaTime * 10f
                );
        }
        else
        {
            bobTimer = 0f;
            playerCamera.localPosition =
                Vector3.Lerp(
                    playerCamera.localPosition,
                    cameraStartPosition,
                    Time.deltaTime * 10f
                );
        }
    }
}