using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemy : MobileEnemy
{
    bool IsAscended = false;
    public GameObject waterProjectile;
    public Transform topOfWaterfall;
    public Transform bottomOfWaterfall;
    [SerializeField] float fallSpeed = 100f;
    [SerializeField] float climbSpeed = 10f;

    public override void NetworkedStart()
    {
        base.NetworkedStart();
        topOfWaterfall = GameObject.Find("TOP").transform;
        bottomOfWaterfall = GameObject.Find("BOTTOM").transform;
        if (IsClient)
        {
            myAgent.enabled = false;
        }
    }

    protected override void SearchForPlayer()
    {
        base.SearchForPlayer();
        if (playerInRange && !lockedIn)
        {
            myAgent.speed = climbSpeed;
            target = topOfWaterfall;
            ChasePlayer(target.position);
        }
    }

    protected override void AI()
    {
        if ((myAgent.destination - transform.position).magnitude < 0.5 && myAgent.destination == topOfWaterfall.position)
        {
            ChangeModel();
        }
        if ((myAgent.destination - transform.position).magnitude < 0.5 && canAttack && myAgent.destination == bottomOfWaterfall.position && playerInRange)
        {
            for (int i = 0; i < 10; i++)
            {
                Shoot(i);
            }
            StartCoroutine(AttackCooldown());
        }
    }

    void Shoot(int i)
    {
        Vector3 dir = Vector3.RotateTowards(transform.forward, transform.right, 36 * i, 0).normalized;
        Vector3 pos = transform.position + attackRadius * dir;
        GameObject b = MyCore.NetCreateObject(4, Owner, pos);
        b.GetComponent<Projectile>().owner = gameObject;
        b.transform.forward = dir;
    }

    public override void Shot(GameObject source)
    {
        myAgent.speed = fallSpeed;
        target = bottomOfWaterfall;
        ChasePlayer(target.position);
        StartCoroutine(LockIn());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.gameObject.tag == "Player" && myAgent.destination == topOfWaterfall.position)
        {
            Player p = other.GetComponent<Player>();
            p.Damage(stats[StatType.ATK], transform);
        }
    }

    void ChangeModel() {
        Debug.Log("Model change");
    }
}
