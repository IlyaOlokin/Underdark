using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public bool MustAggro { get; private set; } = true;
    private List<Damage> damages = new();

    public DamageInfo() { }
    public DamageInfo(bool mustAggro)
    {
        MustAggro = mustAggro;
    }
    
    public void AddDamage(int minDamage, int maxDamage, DamageType damageType = DamageType.Physic, float multiplier = 1)
    {
        damages.Add(new Damage(minDamage, maxDamage, damageType, multiplier));
    }
    
    public void AddDamage(int damage, DamageType damageType = DamageType.Physic, float multiplier = 1)
    {
        damages.Add(new Damage(damage, damage, damageType, multiplier));
    }

    public List<Damage> GetDamages()
    {
        return damages;
    }
}
