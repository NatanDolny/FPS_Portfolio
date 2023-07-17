using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 anchor;
    public Vector2 rand_range = new Vector2(-3, 3);

    public Transform[] points;
    private int destinationPoint;


    public float max_timer;
    private float timer;
    public bool online = false;

    private void Awake()
    {
        anchor = transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        timer = max_timer;
    }

    private void FixedUpdate()
    {
        /*if(online)
        {
            if(timer <= 0)
            {
                Vector3 destination = anchor + new Vector3(Random.Range(rand_range.x, rand_range.y), 0, Random.Range(rand_range.x, rand_range.y));
                agent.SetDestination(destination);
                timer = max_timer;
                // Big roaming
                // /*anchor = transform.position;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }*/
        if (online)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextPoint();
            }
        }
    }

    private void GoToNextPoint()
    {
        if (points.Length == 0)
        {
            return;
        }
        agent.destination = points[destinationPoint].position;

        destinationPoint = (destinationPoint + 1) % points.Length;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

}
