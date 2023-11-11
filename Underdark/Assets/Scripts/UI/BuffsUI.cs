using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private float xOffset;
    [SerializeField] private BuffIcon buffIconPref;
    private List<BuffIcon> buffIcons = new();
    
    public void Init(Player player)
    {
        this.player = player;

        this.player.OnBuffReceive += ReceiveBuff;
        this.player.OnBuffLoose += LooseBuff;
    }

    private void ReceiveBuff(IBuff buff)
    {
        buffIcons.Add(Instantiate(buffIconPref, transform.position, Quaternion.identity, transform));
        UpdateBuffsIcons();
    }

    private void LooseBuff(IBuff buff)
    {
        foreach (var buffIcon in buffIcons)
        {
            if (buffIcon.Buff == buff)
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
            buffIcons[i].transform.localPosition = new Vector3(i * xOffset, 0);
            buffIcons[i].SetData(player.Buffs[i]);
        }
    }
}
