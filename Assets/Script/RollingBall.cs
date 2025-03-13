using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    public float force = 10f;
    public float cooldown = 1f;
    private float cooldownTimer = 0f;
    public float searchRadius = 10f;
    private Rigidbody rb;

    void Start()
    {
        // 获取 Rigidbody 组件
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        cooldownTimer -= Time.fixedDeltaTime;

        if (cooldownTimer <= 0f)
        {
            GameObject nearestPlayer = FindNearestPlayer();
            if (nearestPlayer != null)
            {
                ApplyImpulse(nearestPlayer.transform);
            }
            else
            {
                rb.velocity = Vector3.zero; 
            }

            cooldownTimer = cooldown;
        }

    }

    GameObject FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
          
            if (distance < nearestDistance && distance <= searchRadius)
            {
                nearestDistance = distance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }

    void ApplyImpulse(Transform target)
    {
     
        Vector3 direction = (target.position - transform.position).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
