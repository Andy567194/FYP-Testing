using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
//using static UnityEditor.Experimental.GraphView.GraphView;
using Fusion;
using Fusion.Addons.Physics;

public class EnemyAI : NetworkBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask GroundMesh, PlayerTarget;

    [Networked] public float health { get; set; }
    [Networked] public Vector3 walkPoint { get; set; }
    [Networked] public bool walkPointSet { get; set; } = false;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    [Networked] public bool playerInSightRange { get; set; } = false;
    //[Networked] public bool playerInAttackRange { get; set; } = false;
    //[Networked] public float attackCooldownTimer { get; set; } = 0f;
    Animator animator;

    public override void Spawned()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (Object.HasStateAuthority)
        {
            // Ensure this object is controlled by the server
            agent.enabled = true;
            animator = GetComponent<Animator>();
        }
        else
        {
            agent.enabled = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;
        GameObject playerObj = FindNearestPlayer();
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        if (player != null)
        {
            //Check for sight and attack range
            playerInSightRange = Vector3.Distance(transform.position, player.position) < sightRange;
            //playerInAttackRange = Vector3.Distance(transform.position, player.position) < attackRange;

        }
        //if (!playerInSightRange && !playerInAttackRange) Patroling();

        //if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        //if (playerInAttackRange && playerInSightRange) AttackPlayer();
        if (!playerInSightRange)
        {
            Patroling();
            if (animator != null)
            {
                animator.SetBool("Chasing", false);
            }
        }
        else
        {
            ChasePlayer();
            animator.SetBool("Chasing", true);
        }
        //if (attackCooldownTimer >= 0)
        //{
        //    attackCooldownTimer -= Runner.DeltaTime;
        //}
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            RPC_SetDestination(walkPoint);
        }

        //Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        //if (Mathf.Abs(transform.position.z - walkPoint.z) < 1f && Mathf.Abs(transform.position.x - walkPoint.x) < 1f)
        if (Vector3.Distance(transform.position, walkPoint) < 4f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        // Generate random offsets
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z + randomZ);

        // Ensure the point is on the NavMesh
        if (Physics.CheckSphere(randomPoint, 2f, GroundMesh))
        {
            if (CanReachDestination(randomPoint))
            {
                walkPoint = randomPoint;
                walkPointSet = true;
            }
        }
    }


    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        RPC_SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(player.position);
        RPC_SetDestination(player.position);
        //transform.LookAt(player);

        /*
        if (attackCooldownTimer <= 0)
        {
            ///Attack code here
            var cube = Runner.Spawn(projectile, transform.position, Quaternion.identity);
            var rb = cube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Ensure Rigidbody is not kinematic
                rb.AddForce(rb.transform.forward * 10, ForceMode.Impulse);
                ObjectTimeout objectTimeout = cube.GetComponent<ObjectTimeout>();
                if (objectTimeout != null)
                {
                    objectTimeout.timeout = 5;
                }
            }
            ///End of attack code
            attackCooldownTimer = timeBetweenAttacks;
        }*/
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    GameObject FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    private bool CanReachDestination(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(targetPosition, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            return true; // The agent can reach the destination
        }
        return false; // The agent cannot reach the destination
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(25);
            }
        }
    }
}
