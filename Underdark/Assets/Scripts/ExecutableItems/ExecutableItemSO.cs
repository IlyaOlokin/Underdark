using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Executable", fileName = "New Executable Item")]

public class ExecutableItemSO : Item
{
    [SerializeField] private ExecutableItem executableItem;

    public void Execute(Unit caster)
    {
        executableItem.Execute(caster);
    }

    public override string[] ToString(Unit owner)
    {
        return executableItem.ToString(owner);
    }
}
