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
    NetworkLinkedList<NetworkObject> bullets { get; }
    [Networked] public bool destroyed { get; set; } = false;
    //ParticleSystem[] particles { get; set; }
    public int maximumBullets = 5;
    Transform turret;
    Quaternion originalRotation;
    float transformResetTimer = 3;
    public float bulletLifetime = 5;
    [SerializeField] float sightDistance = 100;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            if (shootPosition == null)
            {
                shootPosition = transform;
            }
            //particles = GetComponentsInChildren<ParticleSystem>();
            turret = transform.Find("Turret");
            if (turret != null)
            {
                originalRotation = turret.rotation;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (destroyed)
            {
                return;
            }
            if (!active)
            {
                ShooterActivation shooterActivation = FindObjectOfType<ShooterActivation>();
                if (shooterActivation == null)
                {
                    active = true;
                }
                else
                {
                    return;
                }
            }
            cooldownTimer -= Runner.DeltaTime;
            if (cooldownTimer <= 0 && bullets.Count < maximumBullets)
            {
                var cube = Runner.Spawn(item, shootPosition.position, Quaternion.Euler(shootPosition.rotation.eulerAngles));
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
                    ObjectTimeout objectTimeout = cube.GetComponent<ObjectTimeout>();
                    if (objectTimeout != null)
                    {
                        objectTimeout.timeout = bulletLifetime;
                    }
                }
                RPC_PlayParticle();
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
            if (turret != null)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(turret.position, turret.forward, out raycastHit, sightDistance, ~LayerMask.GetMask("Ignore Raycast")) && raycastHit.collider.CompareTag("Player"))
                {
                    Vector3 targetPosition = raycastHit.collider.bounds.center;
                    turret.transform.LookAt(targetPosition);
                }
                else
                {
                    if (turret.transform.rotation != originalRotation)
                    {
                        transformResetTimer -= Runner.DeltaTime;
                    }
                    if (transformResetTimer <= 0)
                    {
                        turret.transform.rotation = originalRotation;
                        transformResetTimer = 3;
                    }
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayParticle()
    {
        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.Play();
        }
    }
}
