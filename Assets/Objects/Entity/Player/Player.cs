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

    [SerializeField] Vector3 cameraOffset = Vector3.zero;
    [SerializeField] float maxCameraRotation = 0f;
    [SerializeField] float minCameraRotation = 320f;
    [SerializeField] float additionalGForce = 4f;

    Vector3 moveDir = Vector3.zero;
    Vector2 mouseDir = Vector2.zero;

    Vector2 input;
    Vector2 mouseInput;

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

        if (flag == "MOU")
        {
            string[] args = value.Split(",");
            mouseDir = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
            if (IsServer)
            {
                SendUpdate("MOU", value);
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
            //transform.forward = Vector3.RotateTowards(transform.forward, (transform.right * moveDir.x).normalized, rotSpeed * Time.deltaTime, 0);
            //myRig.angularVelocity = new Vector3(myRig.angularVelocity.x, moveDir.x * rotSpeed * Time.deltaTime, myRig.angularVelocity.z);
        }
        if (IsClient && !IsLocalPlayer)
        {
            //transform.forward = Vector3.RotateTowards(transform.forward, (transform.right * moveDir.x).normalized, rotSpeed * Time.deltaTime, 0);
        }
        if (IsLocalPlayer)
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = cameraOffset;
            //if (input.magnitude >= new Vector2(moveDir.x, moveDir.y).magnitude)
            //transform.forward = Vector3.RotateTowards(transform.forward, (transform.right * input.x).normalized, rotSpeed * Time.deltaTime, 0);
            //if ((Camera.main.transform.rotation.eulerAngles.x > 320 && Camera.main.transform.rotation.eulerAngles.x <= 360) || (Camera.main.transform.rotation.eulerAngles.x >= 0 && Camera.main.transform.rotation.eulerAngles.x < 20))

            Vector3 previousCamPosition = Camera.main.transform.localPosition;
            Quaternion previousCamRotation = Camera.main.transform.localRotation;

            Camera.main.transform.RotateAround(transform.position, transform.right, -8f * mouseInput.y * rotSpeed * Time.deltaTime);

            Vector3 camRotation = Camera.main.transform.localRotation.eulerAngles;

            if (camRotation.x < minCameraRotation && camRotation.x > maxCameraRotation)
            {
                Camera.main.transform.localPosition = previousCamPosition;
                Camera.main.transform.localRotation = previousCamRotation;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            myRig.AddForce(transform.forward * moveDir.z * speed);
            myRig.AddForce(transform.right * moveDir.x * speed);
            if (moveDir == Vector3.zero)
                myRig.velocity = Vector3.zero + (Vector3.up * myRig.velocity.y);
            myRig.AddTorque(transform.up * mouseDir.x * rotSpeed);
            if (mouseDir.x == 0)
                myRig.angularVelocity = Vector3.zero;

            //Gravity
            myRig.AddForce(-(additionalGForce * myRig.drag - 1f) * Vector3.up);
        }
        if (IsClient && !IsLocalPlayer)
        {
            myRig.AddTorque(transform.up * mouseDir.x * rotSpeed);
            if (mouseDir.x == 0)
                myRig.angularVelocity = Vector3.zero;
        }
        if (IsLocalPlayer)
        {
            if (mouseInput.magnitude >= new Vector2(mouseDir.x, mouseDir.y).magnitude)
            {
                myRig.AddTorque(transform.up * mouseInput.x * rotSpeed);
                if (mouseInput.x == 0)
                    myRig.angularVelocity = Vector3.zero;
            }
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

    public void onMouseMove(InputAction.CallbackContext ev) {
        if (IsLocalPlayer)
        {
            if (ev.started || ev.performed)
            {
                mouseInput = ev.ReadValue<Vector2>();
                SendCommand("MOU", mouseInput.x.ToString() + "," + mouseInput.y.ToString());
            }
            else
            {
                mouseInput = Vector2.zero;
                SendCommand("MOU", "0,0");
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