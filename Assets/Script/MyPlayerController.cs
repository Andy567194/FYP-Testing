using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    public float speed = 5f, mouseSensitivity = 2f, jumpForce = 5f;
    public Camera playerCamera;

    private float verticalRotation = 0f;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCamera == null)
        {
            Camera camera = GetComponentInChildren<Camera>();
            if (camera != null)
            {
                playerCamera = camera;
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * GetComponent<Collider>().bounds.extents.y, 0.5f, LayerMask.GetMask("Ground"));

        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;


        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movement
        float moveX = Input.GetAxis("Horizontal") * speed;
        float moveZ = Input.GetAxis("Vertical") * speed;

        // Jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.y = rb.velocity.y;
        rb.velocity = move;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}