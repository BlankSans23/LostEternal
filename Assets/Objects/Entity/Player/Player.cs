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
    Vector3 moveDir = Vector3.zero;
    public GameObject bullet;

    public int playerNumber;
    public string pName;
    [SerializeField] Material[] playerColors;
    [SerializeField] TextMeshProUGUI playerName;

    [SerializeField] Vector3 cameraOffset = Vector3.zero;

    Vector2 input;

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

        if (flag == "MOV")
        {
            string[] args = value.Split(",");
            moveDir = new Vector3(float.Parse(args[0]), 0, float.Parse(args[1]));
            if (IsServer)
            {
                SendUpdate("MOV", value);
            }
        }

        if (flag == "BUFF")
        {
            string[] args = value.Split(',');
            StatType t = (StatType)int.Parse(args[0]);
            int amount = int.Parse(args[1]);

            Buff(t, amount);
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
        myRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if ( IsServer)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, (transform.right * moveDir.x).normalized, rotSpeed * Time.deltaTime, 0);
            //myRig.angularVelocity = new Vector3(myRig.angularVelocity.x, moveDir.x * rotSpeed * Time.deltaTime, myRig.angularVelocity.z);
            myRig.AddForce(transform.forward * moveDir.z * speed);
        }
        if (IsClient && !IsLocalPlayer)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, (transform.right * moveDir.x).normalized, rotSpeed * Time.deltaTime, 0);
        }
        if (IsLocalPlayer)
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = cameraOffset;
            transform.forward = Vector3.RotateTowards(transform.forward, (transform.right * input.x).normalized, rotSpeed * Time.deltaTime, 0);
        }
    }

    public void onMove(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer)
        {
            if (ev.started || ev.performed)
            {
                input = ev.ReadValue<Vector2>();
                SendCommand("MOV", input.x.ToString() + "," + input.y.ToString());
            }
            else
            {
                input = Vector2.zero;
                SendCommand("MOV", "0,0");
            }
        }
    }

    void Shoot(InputAction.CallbackContext ev) { 
    
    }
    
    public void Interact(InputAction.CallbackContext ev) 
    { 
        if (IsLocalPlayer && ev.started)
        {
            //EV = Event Contest
            Debug.Log("InteractKey");
            
        }
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
    public void Buff(StatType type, int amount) {
        stats[type] += amount;

        if (IsServer)
        {
            SendUpdate("BUFF", ((int)type).ToString() + "," + amount.ToString());
        }
        if (IsClient)
        {
            //Visual Crap
        }
    }

    IEnumerator bulletCD() {
        yield return null;
    }

}