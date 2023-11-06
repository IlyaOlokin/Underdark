using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armor", fileName = "New Armor")]

public class Armor : Item
{
     public int ArmorAmount;

     public override string[] ToString()
     {
          List<string> res = new List<string>();
          res.Add(Requirements.ToString());
          res.Add($"Armor: {ArmorAmount}");
          
          return res.ToArray();
     }
}
