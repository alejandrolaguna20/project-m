using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController PlayerModel;
    public Camera playerCamera;     
    public float moveSpeed = 5f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    private float getCurrentMoveSpeed()
    {
        float isSprinting = sprintAction.ReadValue<float>();
        if (isSprinting == 1) {
            return moveSpeed * 2f;
        }
        return moveSpeed;
    }

    void Update()
    {
        isGrounded = PlayerModel.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to stay grounded
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        
        // set y to 0 so it only rotates left/right
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // TODO: rotate movement of the player model as well

        Vector3 movement = cameraForward * moveInput.y + cameraRight * moveInput.x;
        
        PlayerModel.Move(movement * getCurrentMoveSpeed() * Time.deltaTime);

        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        PlayerModel.Move(velocity * Time.deltaTime);
    }
}