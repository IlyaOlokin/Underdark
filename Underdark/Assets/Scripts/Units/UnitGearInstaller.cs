using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitGearInstaller : MonoBehaviour
{
    [SerializeField] private Item head;
    [SerializeField] private Item body;
    [SerializeField] private Item legs;
    [SerializeField] private Item shield;
    [SerializeField] private Item weapon;
    
    [SerializeField] private List<ExecutableItemSO> executableItems;
    
    [SerializeField] private List<ActiveAbilitySO> activeAbilities;
    [SerializeField] private List<int> activeAbilitiesExp;

    private void Start()
    {
        Unit unit = GetComponent<Unit>();
        
        unit.Inventory.Equipment.Head.SetItem(head);
        unit.Inventory.Equipment.Body.SetItem(body);
        unit.Inventory.Equipment.Legs.SetItem(legs);
        unit.Inventory.Equipment.Shield.SetItem(shield);
        unit.Inventory.Equipment.Weapon.SetItem(weapon);

        var loopsExe = Mathf.Min(unit.Inventory.ExecutableSlots.Count, executableItems.Count);

        for (int i = 0; i < loopsExe; i++)
            unit.Inventory.ExecutableSlots[i].SetItem(executableItems[i]);

        var loopsAA = Mathf.Min(unit.Inventory.EquippedActiveAbilitySlots.Count, activeAbilities.Count);
        for (int i = 0; i < loopsAA; i++)
            unit.Inventory.EquippedActiveAbilitySlots[i].SetItem(activeAbilities[i]);

        for (int i = 0; i < activeAbilitiesExp.Count; i++)
        {
            unit.AddExpToActiveAbility(activeAbilities[i].ActiveAbility.ID, activeAbilitiesExp[i]);
        }
        
        Destroy(this);
    }
}
