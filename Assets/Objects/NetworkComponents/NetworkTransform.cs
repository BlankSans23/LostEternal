using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class NetworkTransform : NetworkComponent
{
    Vector3 syncPosition;
    Vector3 syncRotation;

    float threshhold = 0.05f;
    float tempSpeed = 0f;
    float tempRotSpeed = 0f;

    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "POS")
            {
                syncPosition = NetworkCore.Vector3FromString(value);
                tempSpeed = (syncPosition - transform.position).magnitude / MyId.UpdateFrequency;
            }
            if (flag == "ROT")
            {
                syncRotation = NetworkCore.Vector3FromString(value);
                tempRotSpeed = (syncRotation - transform.rotation.eulerAngles).magnitude / MyId.UpdateFrequency;
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if((transform.position - syncPosition).magnitude > threshhold)
            {
                SendUpdate("POS", transform.position.ToString());
                syncPosition = transform.position;

            }
            if ((transform.rotation.eulerAngles - syncRotation).magnitude > threshhold)
            {
                SendUpdate("ROT", transform.rotation.eulerAngles.ToString());
                syncRotation = transform.rotation.eulerAngles;
            }

            if (IsDirty)
            {
                SendUpdate("POS", transform.position.ToString());
                SendUpdate("ROT", transform.rotation.eulerAngles.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            transform.position = Vector3.MoveTowards(transform.position, syncPosition, tempSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(Vector3.MoveTowards(transform.rotation.eulerAngles, syncRotation, tempRotSpeed * Time.deltaTime));
        }
    }
}
