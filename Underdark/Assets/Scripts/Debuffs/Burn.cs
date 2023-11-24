using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : Debuff
{
    private GameObject currentVisualPrefab;
    private BurnInfo burnInfo;
    private DamageInfo damageInfo = new();
    
    private float dmgTimer;
    
    public void Init(BurnInfo burnInfo, Unit receiver, Unit caster, GameObject visual, Sprite effectIcon)
    {
        this.burnInfo = burnInfo;
        this.receiver = receiver;
        this.caster = caster;
        Icon = effectIcon;
        dmgTimer = burnInfo.DmgDelay;
        Duration = burnInfo.Duration;
        Timer = Duration;
        
        damageInfo.AddDamage(this.burnInfo.Damage, DamageType.Fire);

        currentVisualPrefab = Instantiate(visual, transform.position, Quaternion.identity, transform);
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        Timer -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            if (TryGetComponent(out Enemy enemy))
            {
                var hitColliders = enemy.GetNearbyAllies();
                foreach (var hitCollider in hitColliders)
                {
                    if (!hitCollider.transform.TryGetComponent(out Enemy otherEnemy)) continue;
                    
                    if (Vector2.Distance(transform.position, otherEnemy.transform.position) <=
                        burnInfo.BurnJumpDistance)
                    {
                        otherEnemy.GetBurn(burnInfo, caster, currentVisualPrefab, Icon);
                    }
                }
            }
                    
            receiver.TakeDamage(caster, caster, damageInfo, false);
            dmgTimer = burnInfo.DmgDelay;
        }

        if (Timer <= 0)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        receiver.LooseStatusEffect(this);
        Destroy(currentVisualPrefab);
    }
}
