using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    internal int Health { get; set; }
    public void TakeDamage(int damage);
}
