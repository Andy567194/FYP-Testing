using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlock : MonoBehaviour
{
    public CamRaycast camray;
    private Rigidbody blockRigidbody;
    public float forceAmount = 10f; // The amount of force to apply
    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        blockRigidbody = GetComponent<Rigidbody>();

        if (blockRigidbody == null)
        {
            Debug.LogError("No Rigidbody component found on this GameObject.");
        }
    }

    void Update()
    {
        
        // Check for mouse click or a specific key press (e.g., space bar)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // Check if the ray hits the block
            RaycastHit hit;
            if (Physics.Raycast(GlobalVar.CameraACenter, out hit))
            {
                if (hit.rigidbody == blockRigidbody)
                {
                    // Apply force to the block in the direction of the ray
                    blockRigidbody.AddForce(GlobalVar.CameraACenter.direction * forceAmount, ForceMode.Impulse);
                }
            }
        }
        //blockRigidbody.AddForce(transform.forward, ForceMode.Impulse);
    }
}
