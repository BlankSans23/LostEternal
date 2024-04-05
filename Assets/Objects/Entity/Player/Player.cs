using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : Entity
{

    float aggro;
    float score;
    bool bulletLoaded;
    public Image bulletTimer;
    public GameObject sword;
    public GameObject gun;
    public Image atkBuff;
    public Image defBuff;
    Rigidbody myRig;
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onMove(InputAction.CallbackContext ev)
    {
        
    }

    void Shoot(InputAction.CallbackContext ev) { 
    
    }
    
    void Interact(InputAction.CallbackContext ev) { 
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
    void Buff() { 
    
    }

    IEnumerator bulletCD() {
        yield return null;
    }

}
