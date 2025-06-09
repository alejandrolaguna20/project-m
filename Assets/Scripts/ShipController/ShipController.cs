using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public GameObject player;               // Reference to player to deactivate
    public float moveSpeed = 10f;           // Horizontal move speed
    public float verticalSpeed = 5f;        // Vertical up/down speed

    public bool isPiloting = false;

    private InputAction moveAction;
    private InputAction ascendAction;       // Space to ascend
    private InputAction descendAction;      // Shift to descend

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        ascendAction = InputSystem.actions.FindAction("Jump");   // Space bar
        descendAction = InputSystem.actions.FindAction("Sprint"); // Shift key
    }

    private void Update()
    {
        if (!isPiloting) return;

        Vector2 inputMovement = moveAction.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputMovement.x, 0, inputMovement.y);

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 horizontalMovement = camForward * direction.z + camRight * direction.x;

        float vertical = 0f;
        if (ascendAction.ReadValue<float>() > 0) vertical = 1f;
        else if (descendAction.ReadValue<float>() > 0) vertical = -1f;

        Vector3 verticalMovement = Vector3.up * vertical * verticalSpeed;

        Vector3 finalMovement = (horizontalMovement * moveSpeed + verticalMovement) * Time.deltaTime;
        transform.position += finalMovement;
    }
}
