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
    [SerializeField] private GameObject inventory;
    
    [NonSerialized] public Player player;
    
    private void Awake()
    {
        transform.localScale = new Vector3(1, 1, 1);
        inventoryButton.onClick.AddListener(ToggleInventory);
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
