using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public CharacterController PlayerModel;
    public Camera playerCamera;
    public float moveSpeed = 5f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    
    [Header("Interaction")]
    public BoxCollider interactionHitbox; // Drag your trigger box here
    
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction interactAction;
    
    private Vector3 velocity;
    private bool isGrounded;
    private HashSet<GameObject> treesInRange = new HashSet<GameObject>(); // Track trees in hitbox

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        interactAction = InputSystem.actions.FindAction("Interact");
        
        // Make sure hitbox is set as trigger
        if (interactionHitbox != null)
        {
            interactionHitbox.isTrigger = true;
        }
    }

    // Detect when trees enter the hitbox
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            treesInRange.Add(other.gameObject);
            Debug.Log($"Tree entered range: {other.gameObject.name}");
        }
    }

    // Detect when trees leave the hitbox
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            treesInRange.Remove(other.gameObject);
            Debug.Log($"Tree left range: {other.gameObject.name}");
        }
    }

    private void destroyTrees()
    {
        if (treesInRange.Count == 0)
        {
            Debug.Log("No trees in range to destroy");
            return;
        }

        // Create a copy of the list to avoid modification during iteration
        var treesToDestroy = new List<GameObject>(treesInRange);
        
        foreach (GameObject tree in treesToDestroy)
        {
            if (tree != null) // Check if tree still exists
            {
                Debug.Log($"Destroying tree: {tree.name}");
                Destroy(tree);
            }
        }
        
        // Clear the set since all trees are destroyed
        treesInRange.Clear();
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
            velocity.y = -2f;
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement = cameraForward * moveInput.y + cameraRight * moveInput.x;
        PlayerModel.Move(movement * getCurrentMoveSpeed() * Time.deltaTime);

        if (interactAction.ReadValue<float>() == 1) {
            destroyTrees(); // Changed from destroyTree()
        }

        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        PlayerModel.Move(velocity * Time.deltaTime);
    }
}