using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class CarriableObject : NetworkComponent, Interactable
{
    bool IsPickedUp = false;
    Entity carrier;
    [SerializeField] Vector3 offset;
    [SerializeField] float throwForce = 75f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Interact(Entity source) {
        if (IsServer)
        {
            if (!IsPickedUp)
            {
                if (IsLocalPlayer) {
                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
                carrier = source;
                IsPickedUp = true;
                transform.SetParent(source.gameObject.transform);
                transform.localPosition = offset;
                rb.isKinematic = true;
                SendUpdate("PICKUP", rb.isKinematic.ToString());
            }

            else if (IsPickedUp && source == carrier)
            {
                if (IsLocalPlayer)
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
                transform.SetParent(null, true);
                rb.isKinematic = false;
                rb.AddForce(throwForce * source.transform.forward, ForceMode.Impulse);
                SendCommand("PICKUP", rb.isKinematic.ToString());
                IsPickedUp = false;
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return null;
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "PICKUP" && IsServer)
        {
            rb.isKinematic = bool.Parse(value);
        }
    }

    public override void NetworkedStart()
    {
    }
}
