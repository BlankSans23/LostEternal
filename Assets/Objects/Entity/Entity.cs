using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Entity : NetworkComponent
{
    [Tooltip("HP, ATK, then DEF")]
    [SerializeField] protected StatPage stats;

    [SerializeField] protected float speed = 150f;
    [SerializeField] protected float rotSpeed = 1f;

    [SerializeField] float damageDelay = 0.5f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] bool invincible = false;
    [SerializeField] float iFrameLength = 0.3f;

    protected bool canAttack = true;

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

    protected IEnumerator Attack(Vector3 origin, float radius) {
        if (canAttack)
        {
            StartCoroutine(AttackCooldown());
            yield return new WaitForSeconds(damageDelay);
            RaycastHit[] hits = Physics.SphereCastAll(origin, radius, transform.forward);
            foreach (RaycastHit hit in hits)
            {
                Entity e;
                if (hit.collider.gameObject.TryGetComponent<Entity>(out e))
                    e.Damage(stats[StatType.ATK]);
            }
        }
    }

    public void Damage(int atkStrength) {
        if (!invincible)
        {
            if (atkStrength > stats[StatType.DEF])
                stats[StatType.HP] -= atkStrength - stats[StatType.DEF];
            else
                stats[StatType.HP] -= 1;

            StartCoroutine(InvincibilityTimer());
        }

        if (stats[StatType.HP] < 0)
            Die();
    }

    IEnumerator InvincibilityTimer()
    {
        invincible = true;
        yield return new WaitForSeconds(iFrameLength);
        invincible = false;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public virtual void Die() { 
    //lmao freal????
    //Imagine being dead lol
    }
}

#region STAT_PAGE
//Data Wrapper for Above Class
//Make Stats Indexable for accessors
public enum StatType {HP,ATK,DEF}

[System.Serializable]
public class StatPage
{
    [SerializeField] int[] stats;

    public int this[StatType i]
    {
        get
        {
            return stats[(int)i];
        }
        set
        {
            stats[(int) i] = value;
        }
    }
}
#endregion