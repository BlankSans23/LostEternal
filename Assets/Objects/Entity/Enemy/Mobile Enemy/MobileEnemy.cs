using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileEnemy : Enemy
{
    protected NavMeshAgent myAgent;
    [SerializeField] GameObject teleporter;
    [SerializeField] float tpTime = 3f;

    private void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    protected void ChasePlayer(Vector3 chaseTarget) { 
        if (IsServer)
        {
            myAgent.destination = chaseTarget;
            myAgent.isStopped = false;
        }
    }

    //Fuck C# bro
    protected override void AI()
    {
        base.AI();
    }

    protected override void Die()
    {
        StartCoroutine(ActivateTP());
        base.Die();
    }

    IEnumerator ActivateTP()
    {
        yield return new WaitForSeconds(tpTime);
        teleporter.SetActive(true);
    }

    protected void Stop()
    {
        if (IsServer)
        {
            myAgent.isStopped = true;
        }
    }
}
