using UnityEngine;
using Fusion;

public class Key : NetworkBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Make the key kinematic initially
        }
    }

    public void PickUp()
    {
        if (rb != null)
        {
            rb.isKinematic = true; // Make the key kinematic when picked up
            rb.useGravity = false; // Disable gravity
        }
    }

    public void Drop()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Make the key non-kinematic when dropped
            rb.useGravity = true; // Enable gravity
        }
    }
}