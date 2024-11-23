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
    private float cooldownTimer = 0f;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            cooldownTimer -= Runner.DeltaTime;
            if (cooldownTimer <= 0)
            {
                var cube = Runner.Spawn(item, transform.position, Quaternion.identity);
                var rb = cube.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Ensure Rigidbody is not kinematic
                    rb.AddForce(Vector3.right * force, ForceMode.Impulse);
                }
                cooldownTimer = cooldown;
            }
        }
    }

    public void Shoot()
    {
        if (Object.HasStateAuthority)
        {
            var cube = Runner.Spawn(item, transform.position, Quaternion.identity);
            var rb = cube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Ensure Rigidbody is not kinematic
                Debug.Log("Rigidbody found, applying force.");
                Debug.Log($"Rigidbody isKinematic: {rb.isKinematic}");
                Debug.Log($"Rigidbody mass: {rb.mass}");
                Debug.Log($"Applying force: {Vector3.right * force}");
                rb.AddForce(Vector3.right * force, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("No Rigidbody found on spawned item.");
            }
        }
    }
}
