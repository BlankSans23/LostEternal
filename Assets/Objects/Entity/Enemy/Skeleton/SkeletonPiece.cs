using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPiece : MonoBehaviour, Damageable
{
    Skeleton boss;
    [SerializeField] int hp = 3;
    // Start is called before the first frame update
    void Start()
    {
        boss = transform.parent.GetComponent<Skeleton>();
    }

    public void Damage(int atkStrength, Transform e = null) {
        hp--;

        if (hp <= 0)
            boss.Damage(atkStrength);
    }
}
