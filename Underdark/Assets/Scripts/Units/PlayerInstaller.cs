using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoBehaviour
{
    [SerializeField] private ItemsStorageSO itemsStorageSo;
    [SerializeField] private bool resetPlayer;

    private Player player;
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        
        var data = DataLoader.gameData;
        if (data.CurrenLevel == 0) return;
        
        player.Stats.SetLevel(data.CurrenLevel, data.CurrentExp, data.FreePoints);
        player.Stats.Strength = data.Strenght;
        player.Stats.Dexterity = data.Dexterity;
        player.Stats.Intelligence = data.Intelligence;

        var slots = player.Inventory.GetAllSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetItem(itemsStorageSo.GetItemById(data.InventoryItemsIDs[i]), data.InventoryItemsCounts[i]);
        }

        var exeSlots = player.Inventory.ExecutableSlots;
        for (int i = 0; i < exeSlots.Count; i++)
        {
            exeSlots[i].SetItem(itemsStorageSo.GetItemById(data.ExecutableItems[i]), data.ExecutableItemsCounts[i]);
        }

        player.Inventory.Equipment.Head.SetItem(itemsStorageSo.GetItemById(data.Head));
        player.Inventory.Equipment.Body.SetItem(itemsStorageSo.GetItemById(data.Body));
        player.Inventory.Equipment.Legs.SetItem(itemsStorageSo.GetItemById(data.Legs));
        player.Inventory.Equipment.Weapon.SetItem(itemsStorageSo.GetItemById(data.Weapon));
        player.Inventory.Equipment.Shield.SetItem(itemsStorageSo.GetItemById(data.Shield));
        
        var activeAbilities = player.Inventory.GetAllActiveAbilitySlots();
        for (int i = 0; i < activeAbilities.Length; i++)
        {
            activeAbilities[i].SetItem(itemsStorageSo.GetItemById(data.ActiveAbilities[i]));
        }
        
        var equippedActiveAbilities = player.Inventory.EquippedActiveAbilitySlots;
        for (int i = 0; i < equippedActiveAbilities.Count; i++)
        {
            equippedActiveAbilities[i].SetItem(itemsStorageSo.GetItemById(data.EquipedActiveAbilities[i]));
        }
    }

    private void Start()
    {
        if (resetPlayer)
            player.SetPlayer(true);
        else
            player.SetPlayer(false, LevelTransition.playerHP, LevelTransition.playerMP, LevelTransition.cds);
    }
}
