using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnableGravity : NetworkBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (HasStateAuthority)
        {
            Enable();
        }
    }

    public void Enable()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }
}
