using UnityEngine;
using Fusion;

public class MovingPlatform : NetworkBehaviour
{
    public Vector3 pointA; // Starting position
    public Vector3 pointB; // Ending position
    public float speed = 2f; // Speed of movement

    private Vector3 targetPosition;
    private bool movingToB = false; // Tracks direction
    [Networked] public bool timeStopped { get; set; } = false;
    [Networked] private int playersOnPlatform { get; set; } = 0; // Tracks number of players on platform

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            transform.position = pointA;
            targetPosition = pointA; // Start at pointA by default
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            // Move the platform if not time-stopped
            if (!timeStopped)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Runner.DeltaTime);

                // Update target based on player presence
                if (playersOnPlatform > 0)
                {
                    // Move to pointB if players are on the platform
                    targetPosition = pointB;
                    movingToB = true;
                }
                else if (Vector3.Distance(transform.position, pointA) > 0.1f)
                {
                    // Return to pointA if no players are on it (and not already at pointA)
                    targetPosition = pointA;
                    movingToB = false;
                }
            }
        }
    }

    // Detect when a player enters the platform
    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority && other.CompareTag("Player")) // Ensure the object has the "Player" tag
        {
            playersOnPlatform++;
        }
    }

    // Detect when a player leaves the platform
    private void OnTriggerExit(Collider other)
    {
        if (HasStateAuthority && other.CompareTag("Player"))
        {
            playersOnPlatform = Mathf.Max(0, playersOnPlatform - 1); // Prevent negative count
        }
    }

    public void SetTimeStopped(bool value)
    {
        timeStopped = value;
    }
}