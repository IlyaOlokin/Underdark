using Unity.VisualScripting;
using UnityEngine;

public class RestoreOverTimePotion : Potion
{
    [SerializeField] private int healHPAmount;
    [SerializeField] private int manaRestoreAmount;
    [SerializeField] private float restoreDelay;
    [SerializeField] private float duration;
    private float timer;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        var comp = caster.AddComponent<RestoreOverTimePotion>();
        comp.Init(caster, healHPAmount, manaRestoreAmount, restoreDelay, duration);
    }

    public void Init(Unit caster, int heal, int manaRestore, float delay, float duration)
    {
        this.caster = caster;
        healHPAmount = heal;
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
            if (healHPAmount > 0)      caster.RestoreHP(healHPAmount, true);
            if (manaRestoreAmount > 0) caster.RestoreMP(manaRestoreAmount);
            timer = restoreDelay;
        }

        if (duration <= 0)
            Destroy(this);
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        string restore = (healHPAmount > 0 ? healHPAmount + " HP" : "") +
                         (manaRestoreAmount > 0 ? manaRestoreAmount + " MP" : "");
        res[0] = string.Format(description, restore, duration);
        return res;
    }
}
