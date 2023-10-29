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
    
    private Player player;
    
    public void Init(Player player, GameObject inventoryUI, GameObject characterWindowUI)
    {
        this.player = player;
        inventory = inventoryUI;
        characterWindow = characterWindowUI;
    }
    
    private void Awake()
    {
        transform.localScale = new Vector3(1, 1, 1);
        inventoryButton.onClick.AddListener(ToggleInventory);
        characterButton.onClick.AddListener(ToggleCharacterWindow);
    }

    private void Start()
    {
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
        inventory.SetActive(!inventory.activeSelf);
    }
    
    private void ToggleCharacterWindow()
    {
        characterWindow.SetActive(!characterWindow.activeSelf);
    }
}
