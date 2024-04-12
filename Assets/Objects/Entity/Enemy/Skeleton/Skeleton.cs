using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    [SerializeField] List<SkeletonPiece> skeletonPieces;

    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);

        if (flag == "PIECE" && IsClient)
        {
            Damage(0);
        }
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            SkeletonPiece p;
            if (transform.GetChild(i).TryGetComponent<SkeletonPiece>(out p))
                skeletonPieces.Add(p);
        }
    }

    public override void Damage(int attackStrength, Transform e = null)
    {
        SkeletonPiece p = null;
        if (skeletonPieces.Count > 0)
        {
            p = skeletonPieces[0];
            skeletonPieces.RemoveAt(0);
        }

        if (IsServer)
        {
            if (skeletonPieces.Count <= 0)
                Die();

            SendUpdate("PIECE", "");
        }
        if (IsClient && p != null)
        {
            p.GetComponent<Renderer>().enabled = false;
            p.GetComponent<ParticleSystem>().Play();
        }

        if (p != null)
        {
            p.GetComponent<Collider>().enabled = false;
            p.enabled = false;
        }
    }
}
