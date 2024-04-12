using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class FlameSource : NetworkComponent
{
    public List<GameObject> flames;
    public int flameStrength = 4;
    public float flameSpeed = 0.1f;
    public bool isLit = true;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "EX" && IsClient)
        {
            StartCoroutine(Extinguish());
        }
        if (flag == "FIRE" && IsClient)
        {
            StartCoroutine(Ignite());
        }
    }

    public override void NetworkedStart()
    {
        if (IsClient)
        {
            if (isLit)
            {
                foreach (GameObject flame in flames)
                {
                    flame.GetComponent<ParticleSystem>().Play();
                }
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            flames.Add(transform.GetChild(i).gameObject);
        }
    }

    //Incinerate debris
    public IEnumerator Ignite()
    {
        if (IsServer)
        {
            isLit = false;
            GetComponent<Collider>().enabled = false;
            SendUpdate("FIRE", "");
        }
        if (IsClient)
        {
            foreach (GameObject flame in flames)
            {
                flame.GetComponent<ParticleSystem>().Play();
                yield return new WaitForSeconds(flameSpeed);
            }
            yield return StartCoroutine(Extinguish());
        }
        GetComponent<Rigidbody>().useGravity = false;
        transform.SetParent(null, true);
        this.enabled = false;
    }

    public IEnumerator Extinguish() {
        isLit = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        if (IsServer)
        {
            SendUpdate("EX", "");
        }
        if (IsClient)
        {
            foreach (GameObject flame in flames)
            {
                flame.GetComponent<ParticleSystem>().Stop();
                yield return new WaitForSeconds(flameSpeed);
            }
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (!IsServer)
            return;

        if (other.gameObject.tag == "Debris")
        {
            FlameSource debris = other.collider.GetComponent<FlameSource>();
            if (!debris.isLit && isLit)
            {
                StartCoroutine(Extinguish());
                StartCoroutine(debris.Ignite());
            }
        }
        if (other.gameObject.tag == "Player")
        {
            Player p = other.gameObject.GetComponent<Player>();
            if (isLit)
                p.Damage(flameStrength, transform);
        }
    }
}
