using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Entity : NetworkComponent, Damageable
{
    [Tooltip("HP, ATK, then DEF")]
    [SerializeField] public StatPage stats;

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
                Damageable e;
                if (hit.collider.gameObject.TryGetComponent<Damageable>(out e)) {
                    e.Damage(stats[StatType.ATK], e is Player ? this.transform : null);
                }
            }
        }
    }

    protected void knockback(Transform source, float atkStrength)
    {
        Vector3 knockback = transform.position - source.position;
        knockback = new Vector3(knockback.x, 0, knockback.z).normalized;
        knockback += 0.5f * Vector3.up;

        float knockbackMultiplier = (float)(atkStrength - stats[StatType.DEF]);
        if (knockbackMultiplier < 1)
            knockbackMultiplier = 1;
        knockbackMultiplier *= knockbackForce;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(-rb.GetAccumulatedForce());
        rb.velocity = Vector3.zero;

        rb.AddForce(knockback * knockbackMultiplier, ForceMode.Impulse);
    }

    public virtual void Damage(int atkStrength, Transform e = null) {
        if (IsServer)
        {
            if (!invincible)
            {
                if (atkStrength > stats[StatType.DEF])
                    stats[StatType.HP] -= atkStrength - stats[StatType.DEF];
                else
                    stats[StatType.HP] -= 1;

                if (e != null)
                    knockback(e, atkStrength);

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