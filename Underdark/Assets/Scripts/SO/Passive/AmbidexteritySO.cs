using UnityEngine;

[CreateAssetMenu(menuName = "Passive/Ambidexterity", fileName = "New Ambidexterity")]

public class AmbidexteritySO : PassiveSO
{
    public override string ToString()
    {
        return $"You can equip two one-handed weapons.";
    }
}
