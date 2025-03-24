using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MovingBackandForth : NetworkBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis moveAxis = Axis.X;
    public float speed = 5f;

    private Vector3 direction;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            // Set initial direction based on the selected axis
            switch (moveAxis)
            {
                case Axis.X:
                    direction = Vector3.right;
                    Debug.Log("Moving along X axis");
                    break;
                case Axis.Y:
                    direction = Vector3.up;
                    Debug.Log("Moving along Y axis");
                    break;
                case Axis.Z:
                    direction = Vector3.forward;
                    Debug.Log("Moving along Z axis");
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Move the object along the specified axis
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (HasStateAuthority)
        {
            // Reverse the direction when a collision is detected
            direction = -direction;
        }
    }
}
