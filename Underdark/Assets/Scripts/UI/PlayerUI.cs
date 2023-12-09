using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerUI : MonoBehaviour
{
    public PlayerGearUI gearUI;
    public InventoryUI inventoryUI;
    public CharacterWindowUI characterWindowUI;
    [SerializeField] private StatusEffectUI statusEffectUI;
    
    public void Init(Player player)
    {
        inventoryUI.Init(player);
        characterWindowUI.Init(player);
        statusEffectUI.Init(player);
    }
}
