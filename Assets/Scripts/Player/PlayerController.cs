using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController PlayerModel;
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    
    // Input Action references
    InputAction moveAction;
    
    private Vector3 velocity;

    private void Start()
    {
        // Find the "Move" action
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // Read movement input
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        
        // Convert to 3D movement (X and Z, no Y)
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        
        // Apply movement
        PlayerModel.Move(movement * moveSpeed * Time.deltaTime);
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        PlayerModel.Move(velocity * Time.deltaTime);
    }
}