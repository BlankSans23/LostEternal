using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI tooltipText;

    private void Update()
    {
        transform.forward = (Camera.main.transform.position - transform.position).normalized;
        transform.forward -= transform.forward.y * Vector3.up;
        transform.forward = transform.forward.normalized;
    }

    void OnTriggerStay(Collider other)
    {
        Player p;
        if (other.TryGetComponent<Player>(out p))
        {
            if (p.IsLocalPlayer)
            {
                icon.enabled = true;
                tooltipText.enabled = true;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        Player p;
        if (other.TryGetComponent<Player>(out p))
        {
            if (p.IsLocalPlayer)
            {
                icon.enabled = false;
                tooltipText.enabled = false;
            }
        }
    }


}
