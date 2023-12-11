using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    //Player Stats
    public int CurrenLevel;
    public int CurrentExp;
    public int Strenght;
    public int Dexterity;
    public int Intelligence;
    public int FreePoints;
    
    // Inventory
    public List<string> InventoryItemsIDs = new ();
    public List<int> InventoryItemsCounts = new ();
    public string Head;
    public string Body;
    public string Legs;
    public string Weapon;
    public string Shield;
    
    public List<string> ExecutableItems = new ();
    public List<int> ExecutableItemsCounts = new ();
    
    public List<string> ActiveAbilities = new ();
    public List<string> EquipedActiveAbilities = new ();
    
    // Environment
    public int MaxReachedLevel = 1;
    
    // Money
    public int MoneyCount;
}