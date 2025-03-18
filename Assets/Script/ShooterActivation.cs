using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShooterActivation : NetworkBehaviour
{
    [SerializeField] Shooter[] shooters;
    [SerializeField] FireTurret[] fireTurrets;
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
                if (fireTurrets != null)
                {
                    foreach (FireTurret fireTurret in fireTurrets)
                    {
                        fireTurret.active = true;
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
