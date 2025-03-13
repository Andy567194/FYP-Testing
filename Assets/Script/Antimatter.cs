using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Antimatter : NetworkBehaviour
{
    private Rigidbody rb;
    [Networked] private bool timeStopped { get; set; } = false;
    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (timeStopped)
            {
                rb.isKinematic = false;
            }
            else
            {
                rb.isKinematic = true;
            }
        }
    }

    public void SetTimeStopped(bool timeStopped)
    {
        this.timeStopped = timeStopped;
    }
}
