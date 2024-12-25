using UnityEngine;
using Fusion;

public class Trap : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trap triggered by: " + other.name);

        //if (other.CompareTag("Player"))
        // {
        //   Debug.Log("Player tag detected");

        var player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            Debug.Log("PlayerController component found");
            player.TakeDamage(50);
            Runner.Despawn(Object);
        }
        else
        {
            Debug.LogWarning("PlayerController component not found on the player");
        }
    }
    // else
    //  {
    //      Debug.LogWarning("Collider does not have Player tag");
    //  }
}
