using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Projectile : NetworkComponent
{
    public float speed;
    public GameObject owner;
    public float lifeTime = 10f;

    private void Start()
    {
        StartCoroutine(LifeSpan());
    }

    IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this);
    }
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NetworkRigidBody>().IsServer) {
            GetComponent<NetworkRigidBody>().MyRig.velocity = transform.forward * speed;
        }
        
    }

    public override IEnumerator SlowUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void HandleMessage(string flag, string value)
    {
        throw new System.NotImplementedException();
    }

    public override void NetworkedStart()
    {
        throw new System.NotImplementedException();
    }
}
