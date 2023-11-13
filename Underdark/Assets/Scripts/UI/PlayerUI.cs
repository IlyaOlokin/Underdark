using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerUI : MonoBehaviour
{
    private Player player;

    public InventoryUI inventoryUI;
    public CharacterWindowUI characterWindowUI;
    [FormerlySerializedAs("buffsUI")] [SerializeField] private StatusEffectUI statusEffectUI;
    
    public void Init(Player player)
    {
        this.player = player;
        
        inventoryUI.Init(player);
        characterWindowUI.Init(player);
        statusEffectUI.Init(player);
    }
}
