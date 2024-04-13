using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemy : MobileEnemy
{
    bool IsAscended = false;
    public GameObject waterProjectile;
    public Transform topOfWaterfall;
    public Transform bottomOfWaterfall;
    [SerializeField] Collider dragonLoadingZone;
    [SerializeField] float fallSpeed = 100f;
    [SerializeField] float climbSpeed = 10f;

    public override void NetworkedStart()
    {
        base.NetworkedStart();
        topOfWaterfall = GameObject.Find("TOP").transform;
        bottomOfWaterfall = GameObject.Find("BOTTOM").transform;
        if (IsServer)
        {
            dragonLoadingZone.enabled = false;
        }
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
        if ((myAgent.destination - transform.position).magnitude < 3f && (myAgent.destination - topOfWaterfall.position).magnitude < 1f)
        {
            ChangeModel();
        }
        if ((myAgent.destination - transform.position).magnitude < 3f && canAttack && (myAgent.destination - bottomOfWaterfall.position).magnitude < 1f && playerInRange)
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
        Vector3 tempForward = transform.forward;
        transform.Rotate(Vector3.up, 36 * i);
        Vector3 dir = transform.forward;
        transform.forward = tempForward;
        Vector3 pos = transform.position + attackRadius * dir;

        GameObject b = MyCore.NetCreateObject(10, Owner, pos);
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

    private void OnCollisionrEnter(Collision other)
    {
        if (IsServer && other.gameObject.tag == "Player" && myAgent.destination == topOfWaterfall.position)
        {
            Player p = other.gameObject.GetComponent<Player>();
            p.Damage(stats[StatType.ATK], transform);
        }
    }

    public void GemHit()
    {
        Die();
    }

    void ChangeModel() {
        IsAscended = true;
        GetComponent<Collider>().enabled = false;

        if (IsServer)
        {
            dragonLoadingZone.enabled = true;
            SendUpdate("EVOLVE", "");
        }
        if (IsClient)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
