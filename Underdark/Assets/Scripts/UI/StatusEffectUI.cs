using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StatusEffectUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private float xOffset;
    [SerializeField] private StatusEffectIcon statusEffectIconPref;
    private List<StatusEffectIcon> buffIcons = new();
    
    public void Init(Player player)
    {
        this.player = player;

        this.player.OnStatusEffectReceive += ReceiveStatusEffect;
        this.player.OnStatusEffectLoose += LooseStatusEffect;
    }

    private void ReceiveStatusEffect(IStatusEffect statusEffect)
    {
        buffIcons.Add(Instantiate(statusEffectIconPref, transform.position, Quaternion.identity, transform));
        UpdateBuffsIcons();
    }

    private void LooseStatusEffect(IStatusEffect statusEffect)
    {
        foreach (var buffIcon in buffIcons)
        {
            if (buffIcon.StatusEffect == statusEffect)
            {
                buffIcons.Remove(buffIcon);
                if (buffIcon != null) Destroy(buffIcon.gameObject);
                UpdateBuffsIcons();
                return;
            }
        }
        UpdateBuffsIcons();
    }

    private void UpdateBuffsIcons()
    {
        for (int i = 0; i < player.Buffs.Count; i++)
        {
            if (buffIcons[i] == null) continue;
            buffIcons[i].transform.localPosition = new Vector3(i * xOffset, 0);
            buffIcons[i].SetData(player.Buffs[i]);
        }
    }
}
