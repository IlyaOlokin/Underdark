using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Accessory", fileName = "New Accessory")]

public class AccessorySO : Item, IPassiveHolder
{
    [field:SerializeField] public List<PassiveSO> Passives { get; private set; }

    public override string[] ToString(Unit owner)
    {
        List<string> res = new List<string>();
        res.Add(Requirements.ToString(owner));
          
        return res.ToArray();
    }

    public override string[] ToStringAdditional(Unit owner)
    {
        List<string> res = new List<string>();
          
        foreach (var passive in Passives)
            res.Add(passive.ToString());
          
        return res.ToArray();
    }
}