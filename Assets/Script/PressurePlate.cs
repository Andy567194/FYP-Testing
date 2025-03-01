using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject Door;
    public float pressedHeight = 0.1f; // The height the plate will move down to when pressed
    public float pressSpeed = 1f; // Speed at which the plate moves down
    public float resetSpeed = 2f; // The speed at which the plate will return to its original position
    private Vector3 originalPosition; // The original position of the plate
    private float targetHeight; // The target height for the plate
    private bool isPressed = false; // Whether the plate is currently pressed
    

    private void Start()
    {
        // Store the original position of the plate
        originalPosition = transform.position;
        targetHeight = originalPosition.y; // Initialize target height
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding with the plate is a rigidbody
        if (collision.rigidbody != null)
        {
            // Set target height to pressed height
            targetHeight = originalPosition.y - pressedHeight;
            isPressed = true;
        }

        
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the object exiting the collision is a rigidbody
        if (collision.rigidbody != null)
        {
            isPressed = false;
        }
    }

    private void Update()
    {
        // Move the plate towards the target height
        float step = (isPressed ? pressSpeed : resetSpeed) * Time.deltaTime;
        float newY = Mathf.MoveTowards(transform.position.y, targetHeight, step);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // If not pressed, reset the target height to original position
        if (!isPressed)
        {
            targetHeight = originalPosition.y;
        }
        if (transform.position.y < originalPosition.y )
        {
            Door.SetActive(false);
        }
        else if(transform.position.y >= originalPosition.y)
        {
            Door.SetActive(true);
        }
    }
}
