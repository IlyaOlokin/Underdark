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
    public List<Image> buttonsCD;
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
    }

    private void Update()
    {
        for (int i = 0; i < buttonsCD.Count; i++)
        {
            buttonsCD[i].fillAmount = player.ActiveAbilitiesCD[i] / player.ActiveAbilities[i].cooldown;
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
}
