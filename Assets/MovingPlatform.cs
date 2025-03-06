using UnityEngine;
using Fusion;

public class MovingPlatform : NetworkBehaviour
{
    [Header("Movement Settings")]
    public Vector3 pointA; // Starting position
    public Vector3 pointB; // Ending position
    public float speed = 2f; // Speed of movement

    private Vector3 targetPosition;
    private bool movingToB = true;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            transform.position = pointA;
            targetPosition = pointB;
        }
    }

    void Update()
    {
        if (!HasStateAuthority) return;

        // Move the platform
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the platform reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            movingToB = !movingToB;
            targetPosition = movingToB ? pointB : pointA;
        }
    }
}