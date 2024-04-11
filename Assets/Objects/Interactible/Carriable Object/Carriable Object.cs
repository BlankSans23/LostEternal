using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriableObject : MonoBehaviour, Interactable
{
    bool IsPickedUp;
    Entity carrier;
    [SerializeField] Vector3 offset;

    public void Interact(Entity source) {
        Debug.Log(source.ToString() + " interacted with: Carriable Object");
    }
}
