using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] float targetCooldown = 10f;
    [SerializeField] float aggroTime = 3f;
    [SerializeField] float aggroRange = 10f;



    protected Transform target;
    protected bool lockedIn = false;
    protected bool canTarget = true;
    //not sure how we wanna do Animation[] Attacks

    // Update is called once per frame
    void Update()
    {
        if (IsServer) {
            SearchForPlayer();

            //Technology
            if (target != null)
                AI();
        }
    }

    protected virtual void AI()
    {
        Vector3 targetDir = target.position - transform.position;
        targetDir -= Vector3.up * targetDir.y;

        transform.forward = Vector3.RotateTowards(transform.forward, targetDir.normalized, rotSpeed * Time.deltaTime, 0f);

        if ((target.position - transform.position).magnitude < attackRadius + (attackOrigin.position - transform.position).magnitude)
            StartCoroutine(Attack());
    }

    protected void SearchForPlayer() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, aggroRange, transform.forward, aggroRange, attackLayers);
        
        if (canTarget && !lockedIn && hits.Length > 0)
        {
            StartCoroutine(SearchCooldown());
            Player p = hits[Random.Range(0, hits.Length)].collider.gameObject.GetComponent<Player>();
            target = p.transform;
        }
    }

    new public void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    public void Shot(GameObject source) {
        target = source.transform;
        StartCoroutine(LockIn());
    }

    IEnumerator LockIn()
    {
        lockedIn = true;
        yield return new WaitForSeconds(aggroTime);
        lockedIn = false;
    }

    IEnumerator SearchCooldown()
    {
        canTarget = false;
        yield return new WaitForSeconds(targetCooldown);
        canTarget = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Projectile p;
        if (collision.gameObject.TryGetComponent<Projectile>(out p))
        {
            Shot(p.owner);
            MyCore.NetDestroyObject(p.MyId.NetId);
        }
    }
}
