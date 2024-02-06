using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Executable", fileName = "New Executable Item")]

public class ExecutableItemSO : Item
{
    [field:SerializeField] public ExecutableItem ExecutableItem { get; private set; }

    public bool Execute(Unit caster)
    {
        if (ExecutableItem is Elixir)
            ElixirStaticData.ElixirID = ID;
        
        return ExecutableItem.Execute(caster);
    }

    public override string[] ToString(Unit owner)
    {
        return ExecutableItem.ToString(owner);
    }
    
    public override string[] ToStringAdditional(Unit owner)
    {
        return ExecutableItem.ToStringAdditional();
    }
    
}
