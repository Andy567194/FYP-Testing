using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class Shooter : NetworkBehaviour
{
    public GameObject item; // The prefab to instantiate
    public float force = 10f;
    public float cooldown = 1f;
    [SerializeField]
    private Transform shootPosition;
    [SerializeField] bool bulletUseGravity = true;
    private float cooldownTimer = 0f;
    [Networked]
    public bool active { get; set; } = false;
    [Networked, Capacity(10)]
    public NetworkLinkedList<NetworkObject> bullets { get; }
    [Networked] public bool destroyed { get; set; } = false;
    ParticleSystem particleSystem;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            if (shootPosition == null)
            {
                shootPosition = transform;
            }
        }
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (!active || destroyed)
            {
                return;
            }
            cooldownTimer -= Runner.DeltaTime;
            if (cooldownTimer <= 0 && bullets.Count < 10)
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
                    PlatformRotate90 platformRotate90 = cube.GetComponent<PlatformRotate90>();
                    if (platformRotate90 != null)
                    {
                        platformRotate90.Rotate90();
                    }
                }
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }
                bullets.Add(cube);
                cooldownTimer = cooldown;
            }
            foreach (var bullet in bullets)
            {
                if (bullet == null)
                {
                    bullets.Remove(bullet);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (Object.HasStateAuthority)
        {
            if (other.gameObject.CompareTag("TimeStoppable"))
            {
                destroyed = true;
            }
        }
    }
}
