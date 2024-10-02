using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ProjectileMotion : MonoBehaviour
{
    public float forceMagnitude = 10f;
    public int destroytime = 7;

    // Define the direction of the force
    public Vector3 forceDirection ; // Default direction (Z-axis)

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

        // Apply the defined force in the specified direction
        ApplyForceToSphere();
        Destroy(this,destroytime);
    }

    void ApplyForceToSphere()
    {
        // Normalize the direction to ensure consistent force application
        forceDirection =transform.InverseTransformDirection( new Vector3(1, 0, 0));
        Vector3 normalizedDirection = forceDirection.normalized;


        // Calculate the force vector
        Vector3 force = normalizedDirection * forceMagnitude;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.Impulse);
    }
}
