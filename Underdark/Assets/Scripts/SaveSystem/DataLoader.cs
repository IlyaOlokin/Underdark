using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [Header("File Storage Config")] 
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;
    
    public static GameData gameData;
    
    private static FileDataHandler dataHandler;
    
    void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        LoadStartData();
    }

    private void LoadStartData()
    {
        LoadGame();

        LevelTransition.MaxReachedLevel = gameData.MaxReachedLevel;
    }
    
    private static void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            NewGame();
        }
    }

    public static void SaveGame(Player player)
    {
        gameData = new GameData();

        gameData.CurrenLevel = player.Stats.Level;
        gameData.CurrentExp = player.Stats.CurrentExp;
        gameData.Strenght = player.Stats.Strength;
        gameData.Dexterity = player.Stats.Dexterity;
        gameData.Intelligence = player.Stats.Intelligence;
        gameData.FreePoints = player.Stats.FreePoints;
        
        
        var inventorySlots = player.Inventory.GetAllSlots();
        foreach (var t in inventorySlots)
        {
            gameData.InventoryItemsIDs.Add(t.ItemID);
            gameData.InventoryItemsCounts.Add(t.Amount);
        }
        
        var executableSlots = player.Inventory.ExecutableSlots;
        foreach (var t in executableSlots)
        {
            gameData.ExecutableItems.Add(t.ItemID);
            gameData.ExecutableItemsCounts.Add(t.Amount);

        }

        gameData.Head = player.Inventory.Equipment.Head.ItemID;
        gameData.Body = player.Inventory.Equipment.Body.ItemID;
        gameData.Legs = player.Inventory.Equipment.Legs.ItemID;
        gameData.Weapon = player.Inventory.Equipment.Weapon.ItemID;
        gameData.Shield = player.Inventory.Equipment.Shield.ItemID;
        
        var accessories = player.Inventory.Equipment.Accessories;
        foreach (var t in accessories)
        {
            gameData.Accessories.Add(t.ItemID);
        }

        var activeAbilities = player.Inventory.GetAllActiveAbilitySlots();
        foreach (var t in activeAbilities)
        {
            gameData.ActiveAbilities.Add(t.ItemID);
        }
        
        var equippedActiveAbilities = player.Inventory.EquippedActiveAbilitySlots;
        foreach (var t in equippedActiveAbilities)
        {
            gameData.EquipedActiveAbilities.Add(t.ItemID);
        }

        gameData.MoneyCount = player.Money.GetMoney();
        gameData.MaxReachedLevel = LevelTransition.MaxReachedLevel;
        
        dataHandler.Save(gameData);
    }
    
    public static void NewGame()
    {
        gameData = new GameData();
        LevelTransition.MaxReachedLevel = 1;
    }
}
