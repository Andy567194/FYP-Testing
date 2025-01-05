using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    public bool linear = false;
    [Header("Linear will off use gravity")]

    public float forceMagnitude = 10f;


    public int destroytime = 7;

    // Define the direction of the force
    public Vector3 forceDirection; // Default direction (Z-axis)

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component attached to the sphere
        rb = GetComponent<Rigidbody>();

        // Ensure the Rigidbody is present
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the sphere.");
            return;
        }

        if (linear)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
        AngularMotion();

        Destroy(gameObject, destroytime);
    }

    void FixedUpdate()
    {
        // Rotate to face the direction of velocity
        if (!linear && rb.velocity != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(rb.velocity);
            rb.MoveRotation(rotation);
        }
    }
    void AngularMotion()
    {
        // Normalize the direction to ensure consistent force application
        Vector3 normalizedDirection = transform.InverseTransformDirection(forceDirection);
        normalizedDirection = normalizedDirection.normalized;


        // Calculate the force vector
        Vector3 force = normalizedDirection * forceMagnitude;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.Impulse);

    }


}
