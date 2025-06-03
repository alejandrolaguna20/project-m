using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController PlayerModel;
    public float moveSpeed = 5f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    
    InputAction moveAction;
    InputAction jumpAction;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        isGrounded = PlayerModel.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to stay grounded
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        PlayerModel.Move(movement * moveSpeed * Time.deltaTime);

        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        PlayerModel.Move(velocity * Time.deltaTime);
    }
}