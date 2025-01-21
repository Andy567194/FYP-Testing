using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShooterActivation : NetworkBehaviour
{
    [SerializeField] Shooter[] shooters;
    private void OnTriggerStay(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.tag == "Player")
            {
                if (shooters != null)
                {
                    foreach (Shooter shooter in shooters)
                    {
                        shooter.active = true;
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
            }
        }
    }
}
