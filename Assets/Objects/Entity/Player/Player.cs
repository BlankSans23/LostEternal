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

    [SerializeField] float[] mouseSensitivity;
    [SerializeField] Vector3 cameraOffset = Vector3.zero;
    [SerializeField] float maxDownCameraRotation = 0f;
    [SerializeField] float maxUpCameraRotation = 320f;
    [SerializeField] float additionalGForce = 4f;
    [SerializeField] float jumpForce = 100f;
    [SerializeField] private int lives = 3; // Starting number of lives, adjust as needed
    public Transform respawnPoint; // Point where the player will respawn
    public Animator playerAnimator; // Animator to control player animations?


    [HideInInspector] public int playerNumber;
    [HideInInspector] public string pName;
    [SerializeField] Material[] playerColors;
    [SerializeField] TextMeshProUGUI playerName;

    public GameObject sword;
    public GameObject gun;
    public Image atkBuff;
    public Image defBuff;
    //public bool Jumped;

    Rigidbody myRig;
    Vector3 moveDir = Vector3.zero;
    Vector2 input;
    Vector2 mouseInput;

    [HideInInspector] public float bulletCooldown = 6f;
    float score;
    [HideInInspector] public bool bulletLoaded = true;
    float currentRotation = 0;


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

        if (flag == "JMP")
        {
            /*
            if(IsServer)
            {
                Debug.Log("Jumping");

                Jumped = bool.Parse(value);
                if (Jumped)
                {
                    // Perform the jump action
                    myRig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
                SendUpdate("JMP", value);
            }
            if(IsLocalPlayer)
            {
                Jumped = bool.Parse(value);
            }*/
            if (IsServer)
                myRig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (flag == "INTERACT")
        {
            if (IsServer)
            {
                SendUpdate("Interact", "");
                RaycastHit[] hits = Physics.SphereCastAll(attackOrigin.position, attackRadius, transform.forward);

                foreach (RaycastHit hit in hits)
                {
                    Interactable i;

                    if (hit.collider.TryGetComponent<Interactable>(out i))
                        i.Interact(this);
                }
            }
        }

    }

    public override void NetworkedStart()
    {
        if (IsLocalPlayer)
        {
            playerUI ui = GameObject.FindObjectOfType<playerUI>();
            ui.FindPlayer();
            atkBuff = GameObject.Find("ATKBuff").GetComponent<Image>();
            defBuff = GameObject.Find("DEFBuff").GetComponent<Image>();
            atkBuff.enabled = false;
            defBuff.enabled = false;
            
        }
        base.NetworkedStart();
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
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
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
            shootPos.localPosition = new Vector3(shootPos.localPosition.x, 0, shootPos.localPosition.z);

            float previousRotation = currentRotation;

            //Rotate the camera up and down
            currentRotation += -mouseSensitivity[1] * mouseInput.y * rotSpeed * Time.deltaTime;

            if (currentRotation >= 360)
                currentRotation -= 360;
            if (currentRotation < 0)
                currentRotation += 360;

            //Restore old position and rotation if OOB
            //Vector3 camRotation = head.localRotation.eulerAngles;

            //Define bounds in an easy to read way
            float positiveBounds = maxDownCameraRotation;
            float negativeBounds = 360 - maxUpCameraRotation;

            //If OOB go hard set to nearest bound
            if (currentRotation < negativeBounds && currentRotation > positiveBounds)
            {
                currentRotation = previousRotation;
            }

            head.localRotation = Quaternion.Euler(currentRotation, 0, 0);
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
            //myRig.AddTorque(transform.up * mouseInput.x * rotSpeed);
            transform.forward = Vector3.RotateTowards(transform.forward, transform.right, mouseSensitivity[0] * mouseInput.x * rotSpeed * Time.deltaTime, 0);
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

    

    protected override void Die()
    {
        if (IsServer /*  */) 
        {
            //playerAnimator.SetTrigger("Die");

            StartCoroutine(RespawnPlayer(5f)); // 5 seconds delay before respawning
        }
    }

    IEnumerator RespawnPlayer(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        if (lives > 0)
        {
            // Decrease the player's lives count
            lives--;

            // Respawn the player at the respawn point
            transform.position = respawnPoint.position;

            // Reset player health, status, and other necessary variables here
            
            // Notify other players and clients about the respawn if needed
        }
        else
        {
            // If there are no lives left, handle the case accordingly (e.g., end the game)
            // You can set the player as defeated or put them in spectating mode
            HandleGameOver();
        }
    }

     private void HandleGameOver()
    {
        Debug.Log("Game Over!");
    }
    


    #region CONTROLS
    public void onMove(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer  && !GameObject.FindObjectOfType<DialogueManagers>().InDialogue)
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
        if (IsLocalPlayer && !GameObject.FindObjectOfType<DialogueManagers>().InDialogue)
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
        if (IsLocalPlayer && ev.started && !GameObject.FindObjectOfType<DialogueManagers>().InDialogue)
        {
            SendCommand("ATK", "");
        }
    }

    public void Jumping(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer && !GameObject.FindObjectOfType<DialogueManagers>().InDialogue)
        {
            //Debug.Log("Attempting to jump");

            //If there are problems later, the max distance is hard set and not scalable
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 0.90f))
            {
                //Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
                //Debug.DrawLine(transform.position, hit.point, Color.red, 1.0f);

                //myRig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                // Perform jump action
                SendCommand("JMP", "true");
            }
        }
    }


    public void Shoot(InputAction.CallbackContext ev) { 
        if (IsLocalPlayer && ev.started && bulletLoaded && !GameObject.FindObjectOfType<DialogueManagers>().InDialogue)
        {
            StartCoroutine(bulletCD());
            SendCommand("SHOOT", shootPos.position.ToString() + "|" + shootPos.transform.forward.ToString());
        }
    }
    
    public void Interact(InputAction.CallbackContext ev) 
    { 
        if (IsLocalPlayer && ev.started && !GameObject.FindObjectOfType<DialogueManagers>().InDialogue)
        {
            //EV = Event Contest
            SendCommand("INTERACT", "");
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

    public void Buff(StatType type, int amount) {
        stats[type] += amount;

        if (IsServer)
        {
            SendUpdate("BUFF", ((int)type).ToString() + "," + amount.ToString());
        }
        if (IsLocalPlayer)
        {
            switch (type) {
                case StatType.ATK:
                    atkBuff.enabled = true;
                break;
                case StatType.DEF:
                    defBuff.enabled = true;
                break;
            }
        }
    }

    IEnumerator bulletCD() {
        bulletLoaded = false;
        yield return new WaitForSeconds(bulletCooldown);
        bulletLoaded = true;
    }

}