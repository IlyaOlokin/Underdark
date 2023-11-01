using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/MeleeWeapon", fileName = "New Melee Weapon")]
public class MeleeWeapon : Item
{
    public Damage Damage;
    public int AttackRadius;
    public int AttackDistance;
    [Range(0f, 1f)] public float ArmorPierce;
    public List<DebuffInfo> DebuffInfos;
}
