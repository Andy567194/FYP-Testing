using UnityEngine;
using Fusion;

public class MovingPlatform : NetworkBehaviour
{
    public Vector3 pointA; // Starting position
    public Vector3 pointB; // Ending position
    public float speed = 2f; // Speed of movement

    private Vector3 targetPosition;
    private bool movingToB = true;
    [Networked] public bool timeStopped { get; set; } = false;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            transform.position = pointA;
            targetPosition = pointB;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            // Move the platform
            if (!timeStopped)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Runner.DeltaTime);
            }

            // Switch target position if platform reaches its destination
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                movingToB = !movingToB;
                targetPosition = movingToB ? pointB : pointA;
            }
        }
    }

    public void SetTimeStopped(bool value)
    {
        timeStopped = value;
    }
}