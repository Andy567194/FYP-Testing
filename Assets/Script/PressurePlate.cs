using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PressurePlate : NetworkBehaviour
{
    public GameObject Door;
    public float pressedHeight = 0.1f; // The height the plate will move down to when pressed
    public float pressSpeed = 1f; // Speed at which the plate moves down
    public float resetSpeed = 2f; // The speed at which the plate will return to its original position
    public bool OnOffReverse = false;
    private Vector3 originalPosition; // The original position of the plate
    private float targetHeight; // The target height for the plate
    private bool isPressed = false; // Whether the plate is currently pressed
    [Networked] public bool timeStopped { get; set; } = false;


    public override void Spawned()
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

    public override void FixedUpdateNetwork()
    {
        if (timeStopped)
        {
            return;
        }
        // Move the plate towards the target height
        float step = (isPressed ? pressSpeed : resetSpeed) * Time.deltaTime;
        float newY = Mathf.MoveTowards(transform.position.y, targetHeight, step);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // If not pressed, reset the target height to original position
        if (!isPressed)
        {
            targetHeight = originalPosition.y;
        }
        if (transform.position.y < originalPosition.y)
        {
            if (OnOffReverse)
            {
                Rpc_SetDoorActive(true);
            }
            else
            {
                Rpc_SetDoorActive(false);
            }

        }
        else if (transform.position.y >= originalPosition.y)
        {

            if (OnOffReverse)
            {
                Rpc_SetDoorActive(false);
            }
            else
            {
                Rpc_SetDoorActive(true);
            }
        }
    }

    public void SetTimeStopped(bool timeStopped)
    {
        this.timeStopped = timeStopped;
    }

    [Rpc]
    public void Rpc_SetDoorActive(bool active)
    {
        Door.SetActive(active);
    }
}