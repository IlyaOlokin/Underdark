using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Executable", fileName = "New Executable Item")]

public class ExecutableItemSO : Item
{
    [SerializeField] private ExecutableItem executableItem;

    public bool Execute(Unit caster)
    {
        if (executableItem is Elixir)
            ElixirStaticData.ElixirID = ID;
        
        return executableItem.Execute(caster);
    }

    public override string[] ToString(Unit owner)
    {
        return executableItem.ToString(owner);
    }
}
