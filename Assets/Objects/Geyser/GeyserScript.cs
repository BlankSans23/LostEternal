using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using Unity.VisualScripting;


public class GeyserScript : NetworkComponent
{   
    public float geyserStr = 10f;
    public float geyserDelay = 3f;
    public float geyserTimeActive = 2f;
     bool geyserActive = false;
     bool geyserStart = false;
    public override void HandleMessage(string flag, string value)
    {
        //Send message for animating the geyser
        //rest is done on server

    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        
        yield return null;
    }

    public IEnumerator JumpDelay()
    {
        geyserStart = true;

        //Start launch
        yield return new WaitForSeconds(geyserDelay);
        geyserActive = true;

        //Continue launch - then stop
        yield return new WaitForSeconds(geyserTimeActive);
        geyserActive = false;

        geyserStart = false;
    }

    
    public void OnTriggerEnter(Collider collision)
    {
        //Waiting for sever to commence
        if(IsServer)
        {
            //Checking tag for correctness
            if(collision.gameObject.tag == "Player" && geyserStart == false)
            {
                //Begin Jump Delay for launch
                StartCoroutine(JumpDelay());


            }
        }   

    }

    public void OnTriggerStay(Collider collision)
    {
        if(IsServer)
        {
            if(collision.gameObject.tag == "Player" && geyserActive)
            {
                //Letting people jump as long as the geyser is active
                Rigidbody player = collision.gameObject.GetComponent<Rigidbody>();
                player.AddForce(Vector3.up * geyserStr,ForceMode.Impulse);
                
            }
        } 
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
