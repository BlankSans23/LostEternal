using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileEnemy : Enemy
{
    NavMeshAgent myAgent;

    protected void ChasePlayer(Vector3 chaseTarget) { 
        if (IsServer)
        {
            myAgent.destination = chaseTarget;
            myAgent.isStopped = false;
        }
    }

    private void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    protected void Stop()
    {
        if (IsServer)
        {
            myAgent.isStopped = true;
        }
    }

    protected override void AI()
    {
        if (playerInRange && canAttack)
        {
            ChasePlayer(transform.position);
        }
        else
        {
            Stop();
        }

        if ((target.position - transform.position).magnitude < attackRadius + (attackOrigin.position - transform.position).magnitude)
            StartCoroutine(Attack());
    }
}
