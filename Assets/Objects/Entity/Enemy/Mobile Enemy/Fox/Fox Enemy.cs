using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxEnemy : MobileEnemy
{

    FlameSource[] tails;

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
        if (IsServer && other.gameObject.tag == "Player")
        {
            Player p = other.collider.GetComponent<Player>();
            p.Damage(stats[StatType.ATK], this.transform);
        }
    }
}
