using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSource : MonoBehaviour
{
    public List<GameObject> flames;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            flames.Add(transform.GetChild(i).gameObject);
        }
    }

    void Extinguish() {

        if (flames.Count > 0)
            flames.RemoveAt(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
