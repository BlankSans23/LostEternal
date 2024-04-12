using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    // Start is called before the first frame update
    void Damage(int attackStrength, Transform e = null);
}
