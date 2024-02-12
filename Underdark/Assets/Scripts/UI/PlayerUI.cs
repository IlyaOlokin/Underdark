using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public PlayerGearUI gearUI;
    public InventoryUI inventoryUI;
    public ParamsUI paramsUI;
    public CharacterWindowUI characterWindowUI;
    public PauseWindow menuWindowUI;
    [SerializeField] private StatusEffectUI statusEffectUI;
    
    public void Init(Player player)
    {
        inventoryUI.Init(player);
        paramsUI.Init(player);
        characterWindowUI.Init(player);
        statusEffectUI.Init(player);
    }

    private void Awake()
    {
        inventoryUI.AwakeInit();
        
    }
}
