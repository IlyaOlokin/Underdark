using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShield
{
    private int maxHP;
    private int currentHP;

    private float shieldRadius;

    public EnergyShield(int maxHp, float radius)
    {
        maxHP = maxHp;
        currentHP = maxHP;
        shieldRadius = radius;
    }

    public bool AbsorbDamage(ref int damage, float angleToAttacker)
    {
        if (!(angleToAttacker <= shieldRadius / 2f)) return false;

        if (damage > currentHP)
            damage -= currentHP;
        else
        {
            currentHP -= damage;
            damage = 0;
        }
        
        return true;
    }
}
