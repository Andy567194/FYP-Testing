using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RewindBridge : NetworkBehaviour
{
    [Networked] public bool destroyed { get; set; } = false;
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (destroyed) // 270 is equivalent to -90 in Unity's rotation system
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -90), Time.deltaTime * 2);
            }
            if (!destroyed)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 2);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (HasStateAuthority)
        {
            if (!destroyed)
            {
                if (other.collider.CompareTag("TimeStoppable"))
                {
                    destroyed = true;
                }
            }
        }
    }
    [Rpc]
    public void RPC_Rewind()
    {
        if (HasStateAuthority)
        {
            destroyed = false;
        }
    }

}
