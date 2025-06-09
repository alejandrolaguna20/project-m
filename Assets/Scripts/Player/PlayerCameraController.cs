using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Distance Settings")]
    [SerializeField] private float initialDistance = 20f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float zoomSensitivity = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;

    [Header("Player Rotation")]
    [SerializeField] private bool rotatePlayerWithCamera = true;
    [SerializeField] private float playerRotationSpeed = 10f; // For smooth rotation

    private Camera cam;
    private float currentDistance;
    private float horizontalAngle;
    private float verticalAngle;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        currentDistance = initialDistance;

        if (target == null)
        {
            Debug.LogWarning("No target assigned to OrbitCameraController!");
        }
    }

    private void Update()
    {
        if (target == null) return;

        HandleInput();
        UpdateCameraTransform();
    }

    private void HandleInput()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        currentDistance -= scroll * zoomSensitivity * Time.deltaTime;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            horizontalAngle += mouseDelta.x * mouseSensitivity * Time.deltaTime;
            verticalAngle -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        }
    }

    private void UpdateCameraTransform()
    {
        // Rotate player to face camera direction
        if (rotatePlayerWithCamera && target != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, horizontalAngle, 0);
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, playerRotationSpeed * Time.deltaTime);
        }

        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        Vector3 position = target.position + rotation * Vector3.back * currentDistance;

        transform.position = position;
        transform.LookAt(target);
    }

    // Public method to get camera's forward direction for player movement
    public Vector3 GetCameraForward()
    {
        return Quaternion.Euler(0, horizontalAngle, 0) * Vector3.forward;
    }

    public Vector3 GetCameraRight()
    {
        return Quaternion.Euler(0, horizontalAngle, 0) * Vector3.right;
    }
}