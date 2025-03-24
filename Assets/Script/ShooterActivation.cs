using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShooterActivation : NetworkBehaviour
{
    [SerializeField] private Shooter[] shooters;
    [SerializeField] private FireTurret[] fireTurrets;
    [SerializeField] private GameObject respawnPoint; // Assign this in the Inspector for each region's spawn point

    private void OnTriggerStay(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.tag == "Player")
            {
                // Activate shooters and turrets
                if (shooters != null)
                {
                    foreach (Shooter shooter in shooters)
                    {
                        shooter.active = true;
                    }
                }
                if (fireTurrets != null)
                {
                    foreach (FireTurret fireTurret in fireTurrets)
                    {
                        fireTurret.active = true;
                    }
                }

                // Check player's health and trigger respawn if dead
                PlayerController playerController = other.GetComponent<PlayerController>(); // Assuming your player script is named PlayerHealth
                if (playerController != null)
                {
                    if (respawnPoint != null)
                    {
                        playerController.respawnPoint = respawnPoint.transform;
                    }
                    else
                    {
                        Debug.LogWarning("Respawn point not set for " + gameObject.name);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.tag == "Player")
            {
                if (shooters != null)
                {
                    foreach (Shooter shooter in shooters)
                    {
                        shooter.active = false;
                        shooter.destroyed = false;
                    }
                }
                if (fireTurrets != null)
                {
                    foreach (FireTurret fireTurret in fireTurrets)
                    {
                        fireTurret.active = false;
                    }
                }
            }
        }
    }
}