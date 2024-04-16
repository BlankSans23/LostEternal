using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI tooltipText;
    Player p;

    private void Update()
    {
        if (p != null)
            transform.forward = (p.transform.position - transform.position).normalized;
    }

    void OnTriggerStay(Collider other)
    {
        Player temp = p;
        if (other.TryGetComponent<Player>(out p))
        {
            if (p.IsLocalPlayer)
            {
                icon.enabled = true;
                tooltipText.enabled = true;
            }
            else
                p = temp;
        }
    }
    void OnTriggerExit(Collider other)
    {
        Player temp;
        if (other.TryGetComponent<Player>(out temp))
        {
            if (temp.IsLocalPlayer)
            {
                icon.enabled = false;
                tooltipText.enabled = false;
            }
        }
    }


}
