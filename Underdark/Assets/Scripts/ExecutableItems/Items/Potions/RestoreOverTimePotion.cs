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
    private float timer;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        var comp = caster.AddComponent<RestoreOverTimePotion>();
        comp.Init(caster, healAmount, manaRestoreAmount, restoreDelay, Duration, Icon);
    }

    public void Init(Unit caster, int heal, int manaRestore, float delay, float duration, Sprite icon)
    {
        this.caster = caster;
        healAmount = heal;
        manaRestoreAmount = manaRestore;
        restoreDelay = delay;
        Duration = duration;
        Timer = duration;
        timer = delay;
        Icon = icon;
        caster.ReceiveBuff(this);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        Timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (healAmount > 0)      caster.RestoreHP(healAmount, true);
            if (manaRestoreAmount > 0) caster.RestoreMP(manaRestoreAmount);
            timer = restoreDelay;
        }

        if (Timer <= 0)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        caster.LooseBuff(this);
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
