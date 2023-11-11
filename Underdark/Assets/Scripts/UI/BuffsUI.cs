using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsUI : MonoBehaviour
{
    private Player player;
    
    public void Init(Player player)
    {
        this.player = player;

        this.player.OnBuffReceive += UpdateBuffsIcons;
    }

    private void UpdateBuffsIcons(IBuff buff)
    {
        foreach (var b in player.Buffs)
        {
            
        }
    }
}
