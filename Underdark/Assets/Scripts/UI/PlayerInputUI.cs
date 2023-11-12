using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;


public class PlayerInputUI : MonoBehaviour
{
    public FloatingJoystick joystick;
    public Button shootButton;
    [Header("Ability Buttons")]
    public List<Button> activeAbilityButtons;
    public List<Image> buttonsIcons;
    public List<Image> buttonsCD;
    private List<float> abilitiesCDMax;
    [Header("Inventory")]
    public Button inventoryButton;
    private GameObject inventory;
    [Header("Character")]
    public Button characterButton;
    private GameObject characterWindow;

    [Header("Executable Items")] 
    [SerializeField] public ExecutableSlotsHandler executableSlotsHandler;

    [SerializeField] private GameObject blackOut;
    
    private Player player;
    
    public void Init(Player player, GameObject inventoryUI, GameObject characterWindowUI)
    {
        this.player = player;
        inventory = inventoryUI;
        characterWindow = characterWindowUI;

        inventory.GetComponent<InventoryUI>().blackOut = blackOut;
        characterWindow.GetComponent<CharacterWindowUI>().blackOut = blackOut;
    }
    
    private void Awake()
    {
        transform.localScale = new Vector3(1, 1, 1);
        
        inventoryButton.onClick.AddListener(ToggleInventory);
        characterButton.onClick.AddListener(ToggleCharacterWindow);

    }

    private void Start()
    {
        executableSlotsHandler.Init(player);
        inventory.SetActive(false);
        characterWindow.SetActive(false);
        player.Inventory.OnActiveAbilitiesChanged += UpdateEquippedAbilities;
        player.Inventory.OnExecutableItemChanged += UpdateExecutableSlots;
        
        player.Inventory.OnEquipmentChanged += CheckActiveAbilitiesRequirements;
       
        UpdateEquippedAbilities();
    }

    private void Update()
    {
        for (int i = 0; i < abilitiesCDMax.Count; i++)
        {
            if (abilitiesCDMax[i] == 0)
                buttonsCD[i].fillAmount = 0;
            else
                buttonsCD[i].fillAmount = player.ActiveAbilitiesCD[i] / abilitiesCDMax[i];
        }
    }

    private void UpdateExecutableSlots()
    {
        executableSlotsHandler.RefreshAllSlots();
    }

    private void UpdateEquippedAbilities()
    {
        abilitiesCDMax = new List<float>();
        for (int i = 0; i < player.Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            activeAbilityButtons[i].interactable = !player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty || player.GetWeapon();
            buttonsIcons[i].enabled = !player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty;
            if (player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty)
            {
                abilitiesCDMax.Add(0);
                continue;
            }
            
            ActiveAbility activeAbility = player.Inventory.GetActiveAbility(i);
            abilitiesCDMax.Add(activeAbility.cooldown);
            buttonsIcons[i].sprite = player.Inventory.EquippedActiveAbilitySlots[i].Item.Sprite;
        }

        CheckActiveAbilitiesRequirements();
    }

    private void CheckActiveAbilitiesRequirements()
    {
        for (int i = 0; i < player.Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            if (player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty) continue;
            
            ActiveAbility activeAbility = player.Inventory.GetActiveAbility(i);
            activeAbilityButtons[i].interactable = activeAbility.RequirementsMet(player.GetWeapon());
        }
    }

    private void ToggleInventory()
    {
        characterWindow.SetActive(false);
        inventory.SetActive(!inventory.activeSelf);
        executableSlotsHandler.RefreshAllSlots();
    }
    
    private void ToggleCharacterWindow()
    {
        inventory.SetActive(false);
        characterWindow.SetActive(!characterWindow.activeSelf);
    }

    private void OnDisable()
    {
        player.Inventory.OnActiveAbilitiesChanged -= UpdateEquippedAbilities;
        player.Inventory.OnExecutableItemChanged -= UpdateExecutableSlots;
        
        player.Inventory.OnEquipmentChanged -= CheckActiveAbilitiesRequirements;
    }
}
