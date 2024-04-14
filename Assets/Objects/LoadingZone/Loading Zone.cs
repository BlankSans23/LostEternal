using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZone : MonoBehaviour
{

    [SerializeField] int minPlayers = 2;
    int currentPlayers = 0;
    [SerializeField] Transform teleportLoc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player>().IsServer)
            currentPlayers++;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player>().IsServer)
        {
            if (currentPlayers >= minPlayers)
            {
                currentPlayers++;
                other.gameObject.GetComponent<Rigidbody>().position = teleportLoc.position;
                other.GetComponent<Player>().respawnPoint = teleportLoc;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Player>().IsServer)
            currentPlayers--;
    }
}
