using System.Collections;
using UnityEngine;

public class AccuratePuncture : ActiveAblity, IAttacker
{
    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private float visualDuration;
    [SerializeField] private float endXScale;
    [SerializeField] private float startYScale;
    public Transform Transform => transform;
    
    public override void Execute(Unit caster)
    {
        this.caster = caster;
        damage = Mathf.Min(caster.Stats.Dexterity * damageStatMultiplier, maxDamage);
        
        var target = FindClosestTarget(caster);
        
        if (target == null) return;
        Attack(target.GetComponent<IDamageable>());
        
        StartCoroutine(StartVisual(target.transform));
    }
    IEnumerator StartVisual(Transform target)
    {
        visual.gameObject.SetActive(true);
        visual.position = target.position;
        visual.localScale = new Vector3(0, startYScale);
        float scaleXSpeed =  endXScale / visualDuration;
        float scaleYSpeed = -visual.localScale.y / visualDuration;
        visual.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        
        while (visualDuration > 0)
        {
            visualDuration -= Time.deltaTime;
            visual.localScale += new Vector3(scaleXSpeed * Time.deltaTime, scaleYSpeed * Time.deltaTime);
            yield return null;
        }
    }
    
    public void Attack()
    {
        
    }

    public void Attack(IDamageable damageable)
    {
        if (damageable.TakeDamage(caster, damage))
        {
            foreach (var debuffInfo in debuffInfos)
            {
                debuffInfo.Execute(this, (Unit) damageable, caster);
            }
        }
    }
}
