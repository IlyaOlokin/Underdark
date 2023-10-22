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
    
    [NonSerialized]public GameObject inventory;
    [NonSerialized] public Player player;
    
    public void Init(Player player, GameObject inventoryUI)
    {
        this.player = player;
        inventory = inventoryUI;
    }
    
    private void Awake()
    {
        transform.localScale = new Vector3(1, 1, 1);
        inventoryButton.onClick.AddListener(ToggleInventory);
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
}
