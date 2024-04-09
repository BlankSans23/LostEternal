using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriableObject : MonoBehaviour, Interactable
{
    bool IsPickedUp;

    public void Interact(Entity source) {
        Debug.Log(source.ToString() + " interacted with: Carriable Object");
    }
}
