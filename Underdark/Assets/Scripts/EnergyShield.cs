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

    public bool TakeDamage(Unit owner, Unit sender, IAttacker attacker, UnitNotificationEffect newEffect,
        UnitNotificationEffect unitNotificationEffect, ref int newDamage)
    {
        Vector3 dir = attacker == null ? sender.transform.position : attacker.Transform.position -owner.transform.position;
        var angle = Vector2.Angle(dir, owner.GetLastMoveDir());

        var savedDamage = newDamage;

        if (AbsorbDamage(ref newDamage, angle))
        {
            newEffect.WriteDamage(savedDamage, true);

            if (newDamage > 0)
            {
                var newEffectForES = Object.Instantiate(unitNotificationEffect, owner.transform.position, Quaternion.identity);
                newEffectForES.WriteDamage(savedDamage - newDamage, true);
                owner.LooseEnergyShield();
            }
            else
                return true;
        }

        return false;
    }

    private bool AbsorbDamage(ref int damage, float angleToAttacker)
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
