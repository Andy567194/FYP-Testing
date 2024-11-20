using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Shooter : NetworkBehaviour
{
    public GameObject item; // The prefab to instantiate
    public float force = 10f;
    private float cooldown = 1f;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            cooldown -= Runner.DeltaTime;
            if (cooldown <= 0)
            {
                var cube = Runner.Spawn(item, transform.position, Quaternion.identity);
                cube.GetComponent<Rigidbody>().AddForce(Vector3.right * force);
                cooldown = 1;
            }
        }
    }

    public void Shoot()
    {
        if (Object.HasStateAuthority)
        {
            var cube = Runner.Spawn(item, transform.position, Quaternion.identity);
            cube.GetComponent<Rigidbody>().AddForce(Vector3.right * force);
        }
    }
}
