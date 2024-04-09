using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Player : Entity
{
    [SerializeField] Transform head;
    [SerializeField] Transform shootPos;

    [SerializeField] Vector3 cameraOffset = Vector3.zero;
    [SerializeField] float maxDownCameraRotation = 0f;
    [SerializeField] float maxUpCameraRotation = 320f;
    [SerializeField] float additionalGForce = 4f;

    [HideInInspector] public int playerNumber;
    [HideInInspector] public string pName;
    [SerializeField] Material[] playerColors;
    [SerializeField] TextMeshProUGUI playerName;

    public GameObject sword;
    public GameObject gun;
    public Image atkBuff;
    public Image defBuff;
    public Image bulletTimer;

    Rigidbody myRig;
    Vector3 moveDir = Vector3.zero;
    Vector2 input;
    Vector2 mouseInput;

    float bulletCooldown = 6f;
    float score;
    bool bulletLoaded = true;

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

        if (flag == "ROTATE")
        {
            Vector3 forward = NetworkCore.Vector3FromString(value);
            if (IsServer)
            {
                transform.forward = forward;
                SendUpdate("ROTATE", value);
            }
            if (IsClient && !IsLocalPlayer)
            {
                transform.forward = forward;
            }
        }

        if (flag == "BUFF")
        {
            string[] args = value.Split(',');
            StatType t = (StatType)int.Parse(args[0]);
            int amount = int.Parse(args[1]);

            Buff(t, amount);
        }

        if (flag == "ATK")
        {
            if (IsServer)
                StartCoroutine(Attack());
        }

        if (flag == "SHOOT")
        {
            if (IsServer)
            {
                Debug.Log(value);
                string[] shootDetails = value.Split('|');
                Vector3 pos = NetworkCore.Vector3FromString(shootDetails[0]);
                GameObject b = MyCore.NetCreateObject(4, Owner, pos);
                b.GetComponent<Projectile>().owner = gameObject;
                Vector3 dir = NetworkCore.Vector3FromString(shootDetails[1]);
                b.transform.forward = dir;
            }
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
        //Camera code
        if (IsLocalPlayer)
        {
            //Keep the camera on the local player
            Camera.main.transform.SetParent(head);
            Camera.main.transform.localPosition = cameraOffset;
            shootPos.SetParent(Camera.main.transform);

            Vector3 previousRotation = head.localRotation.eulerAngles;
            //Rotate the camera up and down
            head.Rotate(Vector3.right, -8f * mouseInput.y * rotSpeed * Time.deltaTime);

            //Restore old position and rotation if OOB
            Vector3 camRotation = head.localRotation.eulerAngles;

            //Define bounds in an easy to read way
            float positiveBounds = maxDownCameraRotation;
            float negativeBounds = 360 - maxUpCameraRotation;

            //If OOB go hard set to nearest bound
            if (camRotation.x < negativeBounds && camRotation.x > positiveBounds)
            {
                head.localRotation = Quaternion.Euler(previousRotation);
            }
        }
    }

    //Handle the player controller
    private void FixedUpdate()
    {
        if (IsServer)
        {
            myRig.AddForce(transform.forward * moveDir.z * speed);
            myRig.AddForce(transform.right * moveDir.x * speed);
            if (moveDir == Vector3.zero)
                myRig.velocity = Vector3.zero + (Vector3.up * myRig.velocity.y);

            //Gravity
            myRig.AddForce(-(additionalGForce * myRig.drag - 1f) * Vector3.up);
        }
        if (IsLocalPlayer)
        {
            myRig.AddTorque(transform.up * mouseInput.x * rotSpeed);
            SendCommand("ROTATE", transform.forward.ToString());
        }
    }

    new public void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;

        if (shootPos != null)
            Gizmos.DrawSphere(shootPos.position, 0.1f);
    }

    #region CONTROLS
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
            }
            else
            {
                mouseInput = Vector2.zero;
            }
        }
    }

    public void Attack(InputAction.CallbackContext ev) { 
        if (IsLocalPlayer && ev.started)
        {
            SendCommand("ATK", "");
        }
    }

    public void Shoot(InputAction.CallbackContext ev) { 
        if (IsLocalPlayer && ev.started && bulletLoaded)
        {
            StartCoroutine(bulletCD());
            SendCommand("SHOOT", shootPos.position.ToString() + "|" + shootPos.transform.forward.ToString());
        }
    }
    
    public void Interact(InputAction.CallbackContext ev) 
    { 
        if (IsLocalPlayer && ev.started)
        {
            //EV = Event Contest
            Debug.Log("InteractKey");
            RaycastHit[] hits = Physics.SphereCastAll(attackOrigin.position, attackRadius, transform.forward);

            foreach (RaycastHit hit in hits)
            {
                Interactable i;

                if (hit.collider.TryGetComponent<Interactable>(out i))
                    i.Interact(this);
            }
        }

    }
    #endregion

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
        bulletLoaded = false;
        yield return new WaitForSeconds(bulletCooldown);
        bulletLoaded = true;
    }

}