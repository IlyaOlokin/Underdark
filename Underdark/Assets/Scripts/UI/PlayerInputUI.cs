using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using UnityEngine.UI;


public class PlayerInputUI : MonoBehaviour
{
    public Joystick joystick;
    public Button shootButton;
    public Image shootButtonIcon;
    [Header("Ability Buttons")]
    public List<HoldButton> activeAbilityButtons;
    public List<Image> buttonsIcons;
    public List<Image> buttonsCD;
    public List<TextMeshProUGUI> manaCost;
    public List<TextMeshProUGUI> abilityLevel;
    public List<GameObject> notEnoughManaIndicators;
    private List<float> abilitiesCDMax;
    
    [Header("Inventory")]
    public Button inventoryButton;
    private GameObject gearWindow;
    [Header("Character")]
    public Button characterButton;
    private GameObject characterWindow;
    [Header("Menu")]
    public Button pauseButton;
    private PauseWindow pauseWindow;

    [Header("Executable Items")] 
    [SerializeField] public ExecutableSlotsHandler executableSlotsHandler;

    [Header("Visual")] 
    [SerializeField] private FreePointsIndicator freePointsIndicator;
    [SerializeField] private GameObject blackOut;
    
    private Player player;
    private IInput input;
    
    public void Init(Player player, IInput input, GameObject gearUI, GameObject characterWindowUI, PauseWindow menuWindowUI)
    {
        this.player = player;
        this.input = input;
        gearWindow = gearUI;
        characterWindow = characterWindowUI;
        pauseWindow = menuWindowUI;

        gearWindow.GetComponent<PlayerGearUI>().blackOut = blackOut;
        characterWindow.GetComponent<CharacterWindowUI>().blackOut = blackOut;
        
        freePointsIndicator.Init(player.Stats);
    }
    
    private void Awake()
    {
        transform.localScale = new Vector3(1, 1, 1);
        
        inventoryButton.onClick.AddListener(ToggleInventory);
        characterButton.onClick.AddListener(ToggleCharacterWindow);
        pauseButton.onClick.AddListener(OpenPauseMenu);

    }

    private void Start()
    {
        executableSlotsHandler.Init(player);
        gearWindow.SetActive(false);
        characterWindow.SetActive(false);
        player.Inventory.OnActiveAbilitiesChanged += UpdateEquippedAbilities;
        player.Inventory.OnExecutableItemChanged += UpdateExecutableSlots;
        
        player.Inventory.OnEquipmentChanged += UpdateEquippedAbilities;
        player.Inventory.OnEquipmentChanged += UpdateShootButtonIcon;
        player.OnManaChanged += CheckActiveAbilitiesManaCost;
        player.OnIsSilenceChanged += UpdateEquippedAbilities;
       
        UpdateEquippedAbilities();
        UpdateShootButtonIcon();
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
            ActiveAbility activeAbility = player.Inventory.GetEquippedActiveAbility(i);

            manaCost[i].gameObject.SetActive(!(player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty ||
                                               activeAbility.GetManaCost(
                                                   player.GetExpOfActiveAbility(activeAbility.ID)) == 0));
            
            abilityLevel[i].gameObject.SetActive(!player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty);
            
            if (player.Inventory.EquippedActiveAbilitySlots[i].IsEmpty)
            {
                abilitiesCDMax.Add(0);
                continue;
            }

            abilityLevel[i].text = RomanConverter.NumberToRoman(
                activeAbility.ActiveAbilityLevelSetupSO.GetCurrentLevel(
                    player.GetExpOfActiveAbility(activeAbility.ID)));
            manaCost[i].text = activeAbility.GetManaCost(player.GetExpOfActiveAbility(activeAbility.ID)).ToString();
            abilitiesCDMax.Add(activeAbility.Cooldown.GetValue(player.GetExpOfActiveAbility(activeAbility.ID)));
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
            player.Inventory.GetEquippedActiveAbility(i).GearRequirementsMet(player.GetWeapon());
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
            notEnoughManaIndicators[i].SetActive(mana < activeAbility.GetManaCost(player.GetExpOfActiveAbility(activeAbility.ID)));
        }
    }

    private void ToggleInventory()
    {
        characterWindow.SetActive(false);
        
        input.IsEnabled = gearWindow.activeSelf;
        gearWindow.SetActive(!gearWindow.activeSelf);
        executableSlotsHandler.RefreshAllSlots();
    }
    
    private void ToggleCharacterWindow()
    {
        gearWindow.SetActive(false);
        
        input.IsEnabled = characterWindow.activeSelf;
        characterWindow.SetActive(!characterWindow.activeSelf);
    }

    private void OpenPauseMenu()
    {
        gearWindow.SetActive(false);
        characterWindow.SetActive(false);
        input.IsEnabled = gearWindow.activeSelf;
        
        pauseWindow.GetComponent<PauseWindow>().OpenWindow();
    }

    private void UpdateShootButtonIcon()
    {
        shootButtonIcon.sprite = player.GetWeapon().Sprite;
    }

    private void OnDestroy()
    {
        player.Inventory.OnActiveAbilitiesChanged -= UpdateEquippedAbilities;
        player.Inventory.OnExecutableItemChanged -= UpdateExecutableSlots;
        
        player.Inventory.OnEquipmentChanged -= UpdateEquippedAbilities;
        player.Inventory.OnEquipmentChanged -= UpdateShootButtonIcon;
        player.OnManaChanged -= CheckActiveAbilitiesManaCost;
        player.OnIsSilenceChanged -= UpdateEquippedAbilities;
    }
}
