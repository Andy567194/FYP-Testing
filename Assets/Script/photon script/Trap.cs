using UnityEngine;
using Fusion;

public class Trap : NetworkBehaviour
{
    private void OnCollisionStay(Collision other)
    {
        // Ensure the NetworkObject and Runner are valid
        if (Object == null || Runner == null || !Object.HasStateAuthority)
        {
            Debug.LogWarning("Trap: NetworkObject or Runner is null, or no State Authority");
            return;
        }

        // Check if the colliding object is valid and has the "Player" tag
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("Trap: Collision object or GameObject is null");
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Trap: Player tag detected on {other.gameObject.name}");

            // Try to get the PlayerController component
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(25);
                Debug.Log($"Trap: Dealing 25 damage to {other.gameObject.name}");

                // Despawn the trap
                Runner.Despawn(Object);
            }
            else
            {
                Debug.LogWarning($"Trap: No PlayerController found on {other.gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Trap: Collider does not have Player tag: {other.gameObject.name}");
        }
    }
}