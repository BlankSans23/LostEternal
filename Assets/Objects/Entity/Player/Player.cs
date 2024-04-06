using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Player : Entity
{
    float aggro;
    float score;
    bool bulletLoaded;
    public Image bulletTimer;
    public GameObject sword;
    public GameObject gun;
    public Image atkBuff;
    public Image defBuff;
    Rigidbody myRig;
    public GameObject bullet;

    public int playerNumber;
    public string pName;
    [SerializeField] Material[] playerColors;
    [SerializeField] TextMeshProUGUI playerName;

    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);

        if (flag == "NAME")
        {
            pName = value;
            playerName.text = pName;
        }

        if (flag == "COLOR")
        {
            playerNumber = int.Parse(value);
            GetComponent<MeshRenderer>().material = playerColors[playerNumber];
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if (IsDirty)
            {
                SendUpdate("NAME", pName);
                SendUpdate("COLOR", playerNumber.ToString());
            }

            //IsDirty set to false at the lowest tier
            yield return StartCoroutine(base.SlowUpdate());
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

    private void onMove(InputAction.CallbackContext ev)
    {
        
    }

    void Shoot(InputAction.CallbackContext ev) { 
    
    }
    
    void Interact(InputAction.CallbackContext ev) { 
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
    void Buff() { 
    
    }

    IEnumerator bulletCD() {
        yield return null;
    }

}