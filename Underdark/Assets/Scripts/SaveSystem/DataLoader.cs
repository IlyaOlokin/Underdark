using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [Header("File Storage Config")] 
    [SerializeField] private string fileName;
    [SerializeField] private string metaFileName;
    [SerializeField] private bool useEncryption;
    
    public static GameData GameData;
    public static MetaGameData MetaGameData;
    
    private static FileDataHandler dataHandler;
    
    void Awake()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, metaFileName, useEncryption);
        LoadStartData();
    }
    
    private void LoadStartData()
    {
        LoadGame();

        LevelTransition.MaxReachedFloor = GameData.MaxReachedLevel;
        LevelTransition.TutorialCompleted = GameData.TutorialCompleted;
    }
    
    private void Start()
    {
        AudioManager.Instance.SetMusic(MetaGameData.MusicOn);
        AudioManager.Instance.SetSound(MetaGameData.SoundOn);
    }
    
    private static void LoadGame()
    {
        GameData = dataHandler.Load();
        MetaGameData = dataHandler.MetaLoad();

        if (GameData == null)
        {
            NewGame();
        }
        
        if (MetaGameData == null)
        {
            MetaGameData = new MetaGameData();
        }
    }

    public static void SaveGame(Player player)
    {
        GameData = new GameData();

        GameData.CurrenLevel = player.Stats.Level;
        GameData.CurrentExp = player.Stats.CurrentExp;
        GameData.Strenght = player.Stats.Strength;
        GameData.Dexterity = player.Stats.Dexterity;
        GameData.Intelligence = player.Stats.Intelligence;
        GameData.FreePoints = player.Stats.FreePoints;
        
        
        var inventorySlots = player.Inventory.GetAllSlots();
        foreach (var t in inventorySlots)
        {
            GameData.InventoryItemsIDs.Add(t.ItemID);
            GameData.InventoryItemsCounts.Add(t.Amount);
        }
        
        var executableSlots = player.Inventory.ExecutableSlots;
        foreach (var t in executableSlots)
        {
            GameData.ExecutableItems.Add(t.ItemID);
            GameData.ExecutableItemsCounts.Add(t.Amount);

        }

        GameData.Head = player.Inventory.Equipment.Head.ItemID;
        GameData.Body = player.Inventory.Equipment.Body.ItemID;
        GameData.Legs = player.Inventory.Equipment.Legs.ItemID;
        GameData.Weapon = player.Inventory.Equipment.Weapon.ItemID;
        GameData.Shield = player.Inventory.Equipment.Shield.ItemID;
        
        var accessories = player.Inventory.Equipment.Accessories;
        foreach (var t in accessories)
        {
            GameData.Accessories.Add(t.ItemID);
        }

        var activeAbilities = player.Inventory.GetAllActiveAbilitySlots();
        foreach (var t in activeAbilities)
        {
            GameData.ActiveAbilities.Add(t.ItemID);
        }
        
        var equippedActiveAbilities = player.Inventory.EquippedActiveAbilitySlots;
        foreach (var t in equippedActiveAbilities)
        {
            GameData.EquipedActiveAbilities.Add(t.ItemID);
        }

        GameData.LearnedAbilityIDs = player.ActiveAbilitiesExp.Keys.ToList(); 
        GameData.AbilityExp = player.ActiveAbilitiesExp.Values.ToList(); 

        GameData.MoneyCount = player.Money.GetMoney();
        GameData.MaxReachedLevel = LevelTransition.MaxReachedFloor;
        
        GameData.TutorialCompleted = LevelTransition.TutorialCompleted;
        
        dataHandler.Save(GameData);
    }
    
    public static void SaveMetaData()
    {
        dataHandler.MetaSave(MetaGameData);
    }
    
    public static void NewGame()
    {
        GameData = new GameData();
        LevelTransition.MaxReachedFloor = 1;
        LevelTransition.TutorialCompleted = false;
    }
}
