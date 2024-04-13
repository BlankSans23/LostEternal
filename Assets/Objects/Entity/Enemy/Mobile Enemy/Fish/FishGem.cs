using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class FishGem : NetworkComponent, Damageable
{
    FishEnemy fish;
    ParticleSystem death;

    [SerializeField] LoadingZone dragonLoadingZone;
    [SerializeField] Transform tpLocation;
    [SerializeField] float tpRadius = 100f;

    public void Damage(int attackStrength, Transform e = null)
    {
        if (IsServer)
        {
            GetComponent<Collider>().enabled = false;
            dragonLoadingZone.GetComponent<Collider>().enabled = false;
            SendUpdate("HIT", "");
            StartCoroutine(Tp());
        }
    }

    IEnumerator Tp()
    {
        yield return new WaitForSeconds(3f);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, tpRadius, transform.forward);
        foreach (RaycastHit hit in hits)
        {
            Player p;
            if (hit.collider.TryGetComponent<Player>(out p))
            {
                p.transform.position = tpLocation.position;
            }
        }

        this.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tpRadius);
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "HIT")
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            death.Play();
            this.enabled = false;

        }
    }

    public override void NetworkedStart()
    {
        if (IsServer)
            fish = GameObject.FindObjectOfType<FishEnemy>();
        if (IsClient)
            death = GetComponent<ParticleSystem>();
    }

    public override IEnumerator SlowUpdate()
    {
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
