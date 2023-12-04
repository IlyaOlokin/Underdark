using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.UI;


public class PlayerInputUI : MonoBehaviour
{
    public FloatingJoystick joystick;
    public Button shootButton;
    [Header("Ability Buttons")]
    public List<HoldButton> activeAbilityButtons;
    public List<Image> buttonsIcons;
    public List<Image> buttonsCD;
    public List<TextMeshProUGUI> manaCost;
    public List<GameObject> notEnoughManaIndicators;
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
    private IInput input;
    
    public void Init(Player player, IInput input, GameObject inventoryUI, GameObject characterWindowUI)
    {
        this.player = player;
        this.input = input;
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
        
        player.Inventory.OnEquipmentChanged += UpdateEquippedAbilities;
        player.OnManaChanged += CheckActiveAbilitiesManaCost;
        player.OnIsSilenceChanged += UpdateEquippedAbilities;
       
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

    private void UpdateEquippedAbilities(bool reset = false)
    {
        abilitiesCDMax = new List<float>();
        for (int i = 0; i < player.Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            activeAbilityButtons[i].Button.interactable = ShouldAbilityButtonBeInteractable(i);
            buttonsIcons[i].enabled = !player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty;
            manaCost[i].gameObject.SetActive(!(player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty || player.Inventory.GetEquippedActiveAbility(i).ManaCost == 0));
            
            if (player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty)
            {
                abilitiesCDMax.Add(0);
                continue;
            }
            
            ActiveAbility activeAbility = player.Inventory.GetEquippedActiveAbility(i);

            manaCost[i].text = activeAbility.ManaCost.ToString();
            abilitiesCDMax.Add(activeAbility.cooldown);
            buttonsIcons[i].sprite = player.Inventory.EquippedActiveAbilitySlots[i].Item.Sprite;
        }
        
        CheckActiveAbilitiesManaCost(player.CurrentMana);
    }

    private void UpdateEquippedAbilities()
    {
        UpdateEquippedAbilities(false);
    }

    private bool ShouldAbilityButtonBeInteractable(int i)
    {
        return !player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty && !player.IsSilenced &&
            player.Inventory.GetEquippedActiveAbility(i).RequirementsMet(player.GetWeapon());
    }

    private void CheckActiveAbilitiesManaCost(int mana)
    {
        for (int i = 0; i < player.Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            if (player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty)
            {
                notEnoughManaIndicators[i].SetActive(false);
                continue;
            }
            
            ActiveAbility activeAbility = player.Inventory.GetEquippedActiveAbility(i);
            notEnoughManaIndicators[i].SetActive(mana < activeAbility.ManaCost);
        }
    }

    private void ToggleInventory()
    {
        characterWindow.SetActive(false);
        
        input.IsEnabled = inventory.activeSelf;
        inventory.SetActive(!inventory.activeSelf);
        executableSlotsHandler.RefreshAllSlots();
    }
    
    private void ToggleCharacterWindow()
    {
        inventory.SetActive(false);
        
        input.IsEnabled = characterWindow.activeSelf;
        characterWindow.SetActive(!characterWindow.activeSelf);
    }

    private void OnDisable()
    {
        player.Inventory.OnActiveAbilitiesChanged -= UpdateEquippedAbilities;
        player.Inventory.OnExecutableItemChanged -= UpdateExecutableSlots;
        
        player.Inventory.OnEquipmentChanged -= UpdateEquippedAbilities;
        player.OnManaChanged -= CheckActiveAbilitiesManaCost;
        player.OnIsSilenceChanged -= UpdateEquippedAbilities;
    }
}
