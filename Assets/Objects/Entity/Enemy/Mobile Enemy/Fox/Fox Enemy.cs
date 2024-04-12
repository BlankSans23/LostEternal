using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxEnemy : MobileEnemy
{

    [SerializeField] List<FlameSource> tails;

    public override void NetworkedStart()
    {
        base.NetworkedStart();
        for (int i = 0; i < transform.childCount; i++)
        {
            tails.Add(transform.GetChild(i).GetComponent<FlameSource>());
        }
    }

    protected override void AI()
    {
        if (playerInRange && canAttack)
        {
            ChasePlayer(target.position);
        }
        else
        {
            Stop();
        }

        if ((target.position - transform.position).magnitude < attackRadius + (attackOrigin.position - transform.position).magnitude)
            StartCoroutine(Attack());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsServer)
        {
            if (other.gameObject.tag == "Player")
            {
                Player p = other.collider.GetComponent<Player>();
                p.Damage(stats[StatType.ATK], transform);
            }
            if (other.gameObject.tag == "Debris")
            {
                if (tails.Count > 0)
                {
                    tails[0].OnCollisionEnter(other);
                    tails.RemoveAt(0);
                }
                if (tails.Count <= 0)
                {
                    Die();
                }
            }
        }
    }
}
