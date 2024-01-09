using UnityEngine;

public class PlayerView : MonoBehaviour
{
    // Reference to the player's transform
    public Transform Player;

    // Toggle to hide/show cursor when right mouse button is held down
    public bool CursorHiding;

    [Header("Motion Settings")]
    public float sensitivity;
    public float scrollSpeed; 
    public float minRadius;
    public float maxRadius;

    [Header("Obstacle Avoidance")]
    public float obstacleOffset;
    public float maxVerticalAngle;
    public float minVerticalAngle;

    [Header("Raycast Settings")]
    public LayerMask raycastLayerMask;

    // Internal variables for camera control
    private float alpha, beta, radius;

    void Start()
    {
        // Adjust the near clip plane of the camera for better visibility
        if (GetComponent<Camera>() is Camera mainCamera)
            mainCamera.nearClipPlane = 0.01f;
        else
            Debug.LogError("This script must be attached to a GameObject with a Camera component.");
    }

    void Update()
    {
        // Handle mouse input for camera rotation and zoom
        float mouseX = -Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        // Toggle cursor visibility based on user preference
        if (CursorHiding) CursorHandling();

        // Rotate the camera based on mouse input when right mouse button is held down
        if (Input.GetMouseButton(1))
        {
            alpha += mouseX * sensitivity * Mathf.Deg2Rad;
            beta = Mathf.Clamp(beta + mouseY * sensitivity * Mathf.Deg2Rad, minVerticalAngle * Mathf.Deg2Rad, maxVerticalAngle * Mathf.Deg2Rad);
        }

        // Adjust the camera's distance from the player based on mouse scroll input
        radius = Mathf.Clamp(radius - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, minRadius, maxRadius);

        // Calculate the new camera position in polar coordinates
        Vector3 polarPosition = new Vector3(radius * Mathf.Cos(alpha) * Mathf.Cos(beta), radius * Mathf.Sin(beta), radius * Mathf.Sin(alpha) * Mathf.Cos(beta)) + Player.position;

        // Perform a raycast to handle obstacle avoidance
        if (Physics.Raycast(Player.position, polarPosition - Player.position, out RaycastHit hit, radius, raycastLayerMask))
        {
            // Adjust the camera position if an obstacle is hit
            Vector3 adjustedPosition = hit.point + hit.normal * obstacleOffset;
            float distanceToPlayer = Vector3.Distance(adjustedPosition, Player.position);

            transform.position = distanceToPlayer >= minRadius
                ? adjustedPosition
                : Player.position + (adjustedPosition - Player.position).normalized * minRadius;
        }
        else
        {
            // Set the camera position without obstacles
            transform.position = polarPosition;
        }

        // Make the camera look at the player
        transform.LookAt(Player.position);
    }

    // Function to handle cursor visibility based on mouse button state
    private void CursorHandling()
    {
        Cursor.visible = !Input.GetMouseButton(1);
    }
}