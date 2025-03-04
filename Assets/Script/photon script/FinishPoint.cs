using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is a player
        if (other.CompareTag("Player"))
        {
            // Notify your spawner that the level is complete
            BasicSpawner.Instance.RequestLoadNextLevel();
        }
    }
}