using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RestoreOverTimePotion : Potion
{
    [SerializeField] private int healAmount;
    [SerializeField] private int manaRestoreAmount;
    
    [SerializeField] private float restoreDelay;
    private float restoreTimer;
    
    public override bool Execute(Unit caster)
    {
        base.Execute(caster);
        var comp = caster.AddComponent<RestoreOverTimePotion>();
        comp.Init(caster, healAmount, manaRestoreAmount, restoreDelay, Duration, Icon);
        return true;
    }

    public void Init(Unit caster, int heal, int manaRestore, float delay, float duration, Sprite icon)
    {
        this.caster = caster;
        Icon = icon;
        
        healAmount = heal;
        manaRestoreAmount = manaRestore;
        
        Duration = duration;
        Timer = duration;
        
        restoreDelay = delay;
        restoreTimer = delay;
        
        caster.ReceiveStatusEffect(this);
    }

    private void Update()
    {
        restoreTimer -= Time.deltaTime;
        Timer -= Time.deltaTime;
        if (restoreTimer <= 0)
        {
            if (healAmount > 0)      caster.RestoreHP(healAmount, true);
            if (manaRestoreAmount > 0) caster.RestoreMP(manaRestoreAmount);
            restoreTimer = restoreDelay;
        }

        if (Timer <= 0)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        caster.LooseStatusEffect(this);
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        StringBuilder restore = new StringBuilder();

        if (healAmount > 0 && manaRestoreAmount > 0) restore.Append(healAmount + " HP and " + manaRestoreAmount + " MP");
        else if (healAmount > 0) restore.Append(healAmount + " HP");
        else if (manaRestoreAmount > 0) restore.Append(manaRestoreAmount + " MP");
        
        res[0] = string.Format(description, restore, Duration);
        return res;
    }
}
