using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon")]
public class MeleeWeapon : Item
{
    public Damage Damage;
    public int AttackRadius;
    public int AttackDistance;
}
