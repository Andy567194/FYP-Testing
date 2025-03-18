using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag 'TimeStoppable'
        if (collision.gameObject.CompareTag("TimeStoppable"))
        {
            // Destroy this game object
            Destroy(gameObject);
        }
    }
}
