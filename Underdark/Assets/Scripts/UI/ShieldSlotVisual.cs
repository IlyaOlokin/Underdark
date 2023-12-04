using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShieldSlotVisual : MonoBehaviour
{
    private Inventory inventory;
    private Image image;
    
    [Inject]
    protected void Construct(Player player)
    {
        inventory = player.Inventory;
    }

    private void OnEnable()
    {
        inventory.OnEquipmentChanged += UpdateVisual;
        UpdateVisual();
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void UpdateVisual()
    {
        image.enabled = false;
        if (inventory.Equipment.Weapon.IsEmpty) return;
        
        if (inventory.Equipment.GetWeapon().WeaponHandedType == WeaponHandedType.TwoHanded)
        {
            image.enabled = true;
            image.sprite = inventory.Equipment.GetWeapon().Sprite;
        }
    }
    
    private void OnDisable()
    {
        inventory.OnEquipmentChanged -= UpdateVisual;
    }
}
