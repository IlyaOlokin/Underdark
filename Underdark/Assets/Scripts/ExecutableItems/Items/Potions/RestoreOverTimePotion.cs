using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RestoreOverTimePotion : Potion
{
    [SerializeField] private int healAmount;
    [SerializeField] private int manaRestoreAmount;
    [SerializeField] private float restoreDelay;
    [SerializeField] private float duration;
    private float timer;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        var comp = caster.AddComponent<RestoreOverTimePotion>();
        comp.Init(caster, healAmount, manaRestoreAmount, restoreDelay, duration);
    }

    public void Init(Unit caster, int heal, int manaRestore, float delay, float duration)
    {
        this.caster = caster;
        healAmount = heal;
        manaRestoreAmount = manaRestore;
        restoreDelay = delay;
        this.duration = duration;
        timer = delay;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        duration -= Time.deltaTime;
        if (timer <= 0)
        {
            if (healAmount > 0)      caster.RestoreHP(healAmount, true);
            if (manaRestoreAmount > 0) caster.RestoreMP(manaRestoreAmount);
            timer = restoreDelay;
        }

        if (duration <= 0)
            Destroy(this);
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        StringBuilder restore = new StringBuilder();

        if (healAmount > 0 && manaRestoreAmount > 0) restore.Append(healAmount + " HP and " + manaRestoreAmount + " MP");
        else if (healAmount > 0) restore.Append(healAmount + " HP");
        else if (manaRestoreAmount > 0) restore.Append(manaRestoreAmount + " MP");
        
        res[0] = string.Format(description, restore, duration);
        return res;
    }
}
