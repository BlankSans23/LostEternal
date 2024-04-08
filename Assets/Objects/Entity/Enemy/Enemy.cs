using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] float aggroTime = 3f;
    [SerializeField] float aggroRange = 10f;

    protected Transform target;
    protected bool lockedIn = false;
    //not sure how we wanna do Animation[] Attacks

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (IsServer) {
            if (!lockedIn)
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
    }

    protected void SearchForPlayer() {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, aggroRange, transform.forward);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                Player p = hit.collider.gameObject.GetComponent<Player>();
                target = p.transform;
                break;
            }
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        Projectile p;
        if (collision.gameObject.TryGetComponent<Projectile>(out p))
        {
            Shot(p.owner);
        }
    }
}
