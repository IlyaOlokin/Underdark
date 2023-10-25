using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/MeleeWeapon", fileName = "New Melee Weapon")]
public class MeleeWeapon : Item
{
    public Damage Damage;
    public int AttackRadius;
    public int AttackDistance;
    public List<DebuffInfo> DebuffInfos;
}
