using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armor", fileName = "New Armor")]

public class Armor : Item, IPassiveHolder
{
     public int ArmorAmount;
     
     [field:SerializeField] public List<PassiveSO> Passives { get; private set; }
     
     public override string[] ToString(Unit owner)
     {
          List<string> res = new List<string>();
          res.Add(Requirements.ToString(owner));
          res.Add($"Armor: {ArmorAmount}");
          
          return res.ToArray();
     }
     
     public override string[] ToStringAdditional()
     {
          List<string> res = new List<string>();
          
          foreach (var passive in Passives)
               res.Add(passive.ToString());
          
          return res.ToArray();
     }

}
