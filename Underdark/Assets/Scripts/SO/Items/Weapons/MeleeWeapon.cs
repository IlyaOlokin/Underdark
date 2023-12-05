using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/MeleeWeapon", fileName = "New Melee Weapon")]
public class MeleeWeapon : Item, IPassiveHolder
{
    public Damage Damage;
    public WeaponType WeaponType;
    public WeaponHandedType WeaponHandedType;
    public int AttackRadius;
    public int AttackDistance;
    [Range(0f, 1f)] public float ArmorPierce;
    public List<DebuffInfo> DebuffInfos;
    
    [field:SerializeField] public List<PassiveSO> Passives { get; private set; }
    
    public override string[] ToString()
    {
        List<string> res = new List<string>();
        res.Add(Requirements.ToString());
        res.Add($"Damage: {Damage.ToString()}");
        res.Add($"Radius: {AttackRadius.ToString()}");
        res.Add($"Distance: {AttackDistance.ToString()}");
          
        return res.ToArray();
    }
    
    public override string[] ToStringAdditional()
    {
        List<string> res = new List<string>();

        foreach (var debuffInfo in DebuffInfos)
        {
            res.Add(debuffInfo.ToString());
        }
        
        if (ArmorPierce != 0) res.Add($"Attacks ignore {ArmorPierce * 100}% of target's armor");
        
        foreach (var passive in Passives)
        {
            res.Add(passive.ToString());
        }

        return res.ToArray();
    }
}
