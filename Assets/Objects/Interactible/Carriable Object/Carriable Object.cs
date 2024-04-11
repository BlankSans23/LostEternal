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
                carrier = source;
                IsPickedUp = true;
                transform.SetParent(source.gameObject.transform);
                transform.localPosition = offset;
                rb.isKinematic = true;
                SendUpdate("PICKUP", rb.isKinematic.ToString());
            }

            else if (IsPickedUp && source == carrier)
            {
                transform.SetParent(null, true);
                rb.isKinematic = false;
                rb.AddForce(throwForce * (transform.forward - source.transform.position).normalized, ForceMode.Impulse);
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
