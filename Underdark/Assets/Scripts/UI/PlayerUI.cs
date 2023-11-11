using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private Player player;

    public InventoryUI inventoryUI;
    public CharacterWindowUI characterWindowUI;
    [SerializeField] private BuffsUI buffsUI;
    
    public void Init(Player player)
    {
        this.player = player;
        
        inventoryUI.Init(player);
        characterWindowUI.Init(player);
        buffsUI.Init(player);
    }
}
