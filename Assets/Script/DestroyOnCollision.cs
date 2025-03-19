using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DestroyOnCollision : NetworkBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (HasStateAuthority)
        {
            // Check if the collided object has the tag 'TimeStoppable'
            if (collision.gameObject.CompareTag("TimeStoppable"))
            {
                // Destroy this game object
                Runner.Despawn(gameObject.GetComponent<NetworkObject>());
            }
        }
    }
}
