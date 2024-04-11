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

    [SerializeField] protected LayerMask attackLayers;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] protected float attackRadius = 3f;
    [SerializeField] float damageDelay = 0.5f;
    [SerializeField] bool invincible = false;
    [SerializeField] float iFrameLength = 0.3f;
    [SerializeField] float knockbackForce = 20f;

    [SerializeField] protected Transform attackOrigin;

    protected bool canAttack = true;

    NetworkAnimator anim;

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
        if (flag == "HP" && IsClient)
        {
            stats[StatType.HP] = int.Parse(value);
            if (stats[StatType.HP] <= 0)
                Die();
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (attackOrigin != null)
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }

    protected IEnumerator Attack() {
        if (IsServer && canAttack)
        {
            StartCoroutine(AttackCooldown());
            yield return new WaitForSeconds(damageDelay);
            RaycastHit[] hits = Physics.SphereCastAll(attackOrigin.position, attackRadius,transform.forward, attackRadius, attackLayers);
            foreach (RaycastHit hit in hits)
            {                
                Entity e;
                if (hit.collider.gameObject.TryGetComponent<Entity>(out e)) {
                    e.Damage(stats[StatType.ATK], e is Player ? this : null);
                }
            }
        }
    }

    public virtual void Damage(int atkStrength, Entity e = null) {
        if (IsServer)
        {
            if (!invincible)
            {
                if (atkStrength > stats[StatType.DEF])
                {
                    stats[StatType.HP] -= atkStrength - stats[StatType.DEF];
                    if (e != null)
                        GetComponent<Rigidbody>().AddForce(((transform.position - e.transform.position).normalized + 0.3f * Vector3.up) * (float)(atkStrength - stats[StatType.DEF]) * knockbackForce, ForceMode.Impulse);
                }
                else
                {
                    stats[StatType.HP] -= 1;
                    if (e != null)
                        GetComponent<Rigidbody>().AddForce(((transform.position - e.transform.position).normalized + 0.7f * Vector3.up) * knockbackForce, ForceMode.Impulse);
                }

                StartCoroutine(InvincibilityTimer());
            }

            if (stats[StatType.HP] <= 0)
                Die();

            SendUpdate("HP", stats[StatType.HP].ToString());
        }
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

    protected virtual void Die() { 
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