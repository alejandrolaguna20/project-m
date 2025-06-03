using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController PlayerModel;
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    
    InputAction moveAction;
    
    private Vector3 velocity;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        
        PlayerModel.Move(movement * moveSpeed * Time.deltaTime);
        
        velocity.y += gravity * Time.deltaTime;
        PlayerModel.Move(velocity * Time.deltaTime);
    }
}