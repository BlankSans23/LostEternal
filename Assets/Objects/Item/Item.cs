using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] StatType increaseType;
    [SerializeField] int buffAmount;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player p = collision.gameObject.GetComponent<Player>();

            if (p.IsLocalPlayer)
            {
                p.SendCommand("BUFF", ((int)increaseType).ToString() + "," + buffAmount.ToString());
                GetComponent<MeshRenderer>().enabled = false;
                this.enabled = false;
            }
        }
    }
}
