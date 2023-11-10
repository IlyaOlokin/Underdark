using UnityEngine;

[CreateAssetMenu(menuName = "Item/ActiveAbility", fileName = "New Active Ability")]
public class ActiveAbilitySO : Item
{
    public ActiveAbility ActiveAbility;
    
    public override string[] ToString()
    {
        return ActiveAbility.ToString();
    }
    
    public override string[] ToStringAdditional()
    {
        return ActiveAbility.ToStringAdditional();
    }
}