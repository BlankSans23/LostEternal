using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float speed;
    public GameObject owner;


    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NetworkRigidBody>().IsServer) {
            GetComponent<NetworkRigidBody>().MyRig.velocity = transform.forward * speed;
        }
        
    }
}
