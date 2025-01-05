using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class Shooter : NetworkBehaviour
{
    public GameObject item; // The prefab to instantiate
    public float force = 10f;
    public float cooldown = 1f;
    [SerializeField]
    private Transform shootPosition;
    [SerializeField] bool bulletUseGravity = true;
    private float cooldownTimer = 0f;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            if (shootPosition == null)
            {
                shootPosition = transform;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            cooldownTimer -= Runner.DeltaTime;
            if (cooldownTimer <= 0)
            {
                var cube = Runner.Spawn(item, shootPosition.position, Quaternion.Euler(this.shootPosition.rotation.eulerAngles));
                var rb = cube.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Ensure Rigidbody is not kinematic
                    if (!bulletUseGravity)
                    {
                        rb.useGravity = false;
                    }
                    rb.AddForce(rb.transform.forward * force, ForceMode.Impulse);
                }
                cooldownTimer = cooldown;
            }
        }
    }
}
