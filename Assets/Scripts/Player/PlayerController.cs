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
    public bool disableController = false;
    private ShipController nearbyShip;

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
    public void OnHitboxTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            treesInRange.Add(other.gameObject);
            Debug.Log($"Tree entered range: {other.gameObject.name}");
        }
        else if (other.CompareTag("Ship"))
        {
            nearbyShip = other.GetComponent<ShipController>();
            Renderer[] renderers = PlayerModel.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }
            // attach camera to ship
            playerCamera.transform.SetParent(nearbyShip.transform);
            playerCamera.transform.localPosition = new Vector3(0, 2, -5);
            playerCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            PlayerModel.transform.SetParent(nearbyShip.transform);
            PlayerModel.transform.localPosition = new Vector3(0, 0, 0);
            PlayerModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
            disableController = true;
            nearbyShip.isPiloting = true;
        }
    }

    // Detect when trees leave the hitbox
    public void OnHitboxTriggerExit(Collider other)
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
        if (isSprinting == 1)
        {
            return moveSpeed * 2f;
        }
        return moveSpeed;
    }

    void Update()
    {
        if (disableController)
        {
            if (interactAction.WasPressedThisFrame() && nearbyShip != null)
            {
                Renderer[] renderers = PlayerModel.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
                playerCamera.transform.SetParent(null);
                PlayerModel.transform.SetParent(null);
                PlayerModel.transform.position = nearbyShip.transform.position + new Vector3(0, 1, 0);
                PlayerModel.transform.rotation = Quaternion.Euler(0, nearbyShip.transform.eulerAngles.y, 0);
                disableController = false;
                nearbyShip.isPiloting = false;
                nearbyShip = null;
                return;
            }
            return;
        }
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

        if (interactAction.ReadValue<float>() == 1)
        {
            destroyTrees();
        }

        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        PlayerModel.Move(velocity * Time.deltaTime);
    }
}