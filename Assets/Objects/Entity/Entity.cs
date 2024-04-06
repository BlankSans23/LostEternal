using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Entity : NetworkComponent
{
    int attack;
    int defense;
    public int hp;
    float speed;
    float rotSpeed;
    float damageDelay;
    float attackCooldown;
    bool invincible;
    float iFrameLength;
    NetworkAnimator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        if (IsServer)
        {
            if (IsDirty)
            {
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }

    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        
    }

    IEnumerator Attack(Vector3 origin, float radius) {
        yield return null;
    }

    void Damage(int atkStrength) { 
    
    }

    IEnumerator InvincibilityTimer()
    {
        yield return null;
    }

    void Die() { 
    //lmao freal????
    }
}
