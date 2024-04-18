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
    protected bool playerInRange = false;
    //not sure how we wanna do Animation[] Attacks

    // Update is called once per frame
    void Update()
    {
        if (IsServer) {
            //Technology
            if (target != null)
                AI();

            SearchForPlayer();
        }
    }

    protected virtual void AI()
    {
        if (playerInRange)
        {
            Vector3 targetDir = target.position - transform.position;
            targetDir -= Vector3.up * targetDir.y;

            transform.forward = Vector3.RotateTowards(transform.forward, targetDir.normalized, rotSpeed * Time.deltaTime, 0f);

            if ((target.position - transform.position).magnitude < attackRadius + (attackOrigin.position - transform.position).magnitude)
                StartCoroutine(Attack());
        }
    }

    protected virtual void SearchForPlayer() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, aggroRange, transform.forward, aggroRange, attackLayers);

        if (hits.Length > 0)
            playerInRange = true;
        else
            playerInRange = false;

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

    public virtual void Shot(GameObject source) {
        target = source.transform;
        StartCoroutine(LockIn());
    }

    protected IEnumerator LockIn()
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

    protected override void Die()
    {
        base.Die();

        Collider c;
        if (TryGetComponent<Collider>(out c))
            c.enabled = false;

        if (IsServer)
        {
            GameObject.FindObjectOfType<GameMaster>().DefeatEnemy();
        }
        if (IsClient)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        this.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            Projectile p;
            if (collision.gameObject.TryGetComponent<Projectile>(out p))
            {
                Shot(p.owner);
                MyCore.NetDestroyObject(p.MyId.NetId);
            }
        }
    }
}
