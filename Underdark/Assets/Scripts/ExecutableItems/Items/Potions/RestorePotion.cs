using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RestorePotion : Potion
{
    [SerializeField] private int healAmount;
    [SerializeField] private int manaRestoreAmount;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        if (healAmount > 0)        caster.RestoreHP(healAmount, true);
        if (manaRestoreAmount > 0) caster.RestoreMP(manaRestoreAmount);
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        StringBuilder restore = new StringBuilder();

        if (healAmount > 0 && manaRestoreAmount > 0) restore.Append(healAmount + " HP and " + manaRestoreAmount + " MP");
        else if (healAmount > 0) restore.Append(healAmount + " HP");
        else if (manaRestoreAmount > 0) restore.Append(manaRestoreAmount + " MP");
        
        res[0] = string.Format(description, restore);
        return res;
    }
}
