using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NETWORK_ENGINE;


public class NPM : NetworkComponent
{
    public string pName;
    public bool isReady;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "READY")
        {
            isReady = bool.Parse(value);
            if(IsServer)
            {
                SendUpdate("READY", value);
            }
            if(IsClient)
            {
                transform.GetChild(2).GetComponent<Toggle>().isOn = isReady;
            }
        }

        if(flag == "NAME")
        {
            pName = value;
            if(IsServer)
            {
                SendUpdate("NAME", value);
            }
            if (IsClient)
            {
                transform.GetChild(1).GetComponent<TMP_InputField>().text = value;
            }
        }
    }
    public void UI_Ready(bool r)
    {
        if(IsLocalPlayer)
        {
            SendCommand("READY", r.ToString());
        }
    }

    public void UI_NameInput(string s)
    {
        if (IsLocalPlayer)
        {
            SendCommand("NAME", s);
        }

    }

    public override void NetworkedStart()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("ReadyUp").transform);
        if (IsClient && !IsLocalPlayer)
        {
            transform.GetChild(1).GetComponent<TMP_InputField>().interactable = false;
            transform.GetChild(2).GetComponent<Toggle>().interactable = false;
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while(IsServer)
        {

            if(IsDirty)
            {
                SendUpdate("NAME", pName);
                SendUpdate("READY", isReady.ToString());

                IsDirty = false;
            }

            yield return new WaitForSeconds(.1f);
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
