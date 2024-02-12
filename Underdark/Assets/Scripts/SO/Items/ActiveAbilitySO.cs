using UnityEngine;

[CreateAssetMenu(menuName = "Item/ActiveAbility", fileName = "New Active Ability")]
public class ActiveAbilitySO : Item
{
    public ActiveAbility ActiveAbility;
    
    public override string[] ToString(Unit owner)
    {
        return ActiveAbility.ToString(owner);
    }
    
    public override string[] ToStringAdditional(Unit owner)
    {
        return ActiveAbility.ToStringAdditional(owner);
    }
}
