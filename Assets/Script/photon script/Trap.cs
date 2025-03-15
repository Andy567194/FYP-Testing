using UnityEngine;
using Fusion;

public class Trap : NetworkBehaviour
{
    private void OnCollisionStay(Collision other)
    {

        //if (other.CompareTag("Player"))
        // {
        //   Debug.Log("Player tag detected");

        var player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(25);
            Runner.Despawn(Object);
        }
    }
    // else
    //  {
    //      Debug.LogWarning("Collider does not have Player tag");
    //  }
}
