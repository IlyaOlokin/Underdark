using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Accessory", fileName = "New Accessory")]

public class Accessory : Item, IPassiveHolder
{
    [field:SerializeField] public List<PassiveSO> Passives { get; private set; }
    
    public override string[] ToString()
    {
        List<string> res = new List<string>();
        res.Add(Requirements.ToString());
          
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
