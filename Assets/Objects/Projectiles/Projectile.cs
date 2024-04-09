using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Projectile : NetworkComponent
{
    public float speed = 10f;
    [SerializeField] float lifeTime = 2f;
    public GameObject owner;

    #region NETWORK_CRAP
    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        if (IsServer)
            StartCoroutine(LifeSpan());
    }

    public override IEnumerator SlowUpdate()
    {
        yield return null;
    }
    #endregion

    IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(lifeTime);
        MyCore.NetDestroyObject(MyId.NetId);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer) {
            GetComponent<Rigidbody>().velocity = transform.forward * speed * Time.deltaTime;
        }
        
    }
}
