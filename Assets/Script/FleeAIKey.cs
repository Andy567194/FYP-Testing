using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeAIKey : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public GameObject Player;
    public GameObject Player2;
    public float DistanceRun = 4.0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.transform.position);
        float distance2 = Vector3.Distance(transform.position, Player2.transform.position);
        if (distance < DistanceRun) { 
            Vector3 dirToPlayer = transform.position - Player.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            agent.SetDestination(newPos);
        }
        if (distance2 < DistanceRun)
        {
            Vector3 dirToPlayer = transform.position - Player2.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            agent.SetDestination(newPos);
        }
    }
}
