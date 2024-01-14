using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BuySellMenu : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;

    private IInventorySlot activeSlot;
    private Player player;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }

    private void Awake()
    {
        buyButton.onClick.AddListener(Buy);
        sellButton.onClick.AddListener(Sell);
    }

    private void Buy()
    {
        if (player.Money.TrySpendMoney(activeSlot.Item.Cost))
        {
            if (player.Inventory.TryAddItem(activeSlot.Item) != 0)
            {
                NotificationManager.Instance.SendNotification(new Notification(null, "Your inventory is already full."));
                player.Money.AddMoney(activeSlot.Item.Cost);
            }
        }
    }

    private void Sell()
    {
        player.Money.AddMoney(activeSlot.Item.Cost);
        player.Inventory.Remove(activeSlot);
    }
    public void UpdateButtons(bool isPlayersSlot, IInventorySlot slot)
    {
        activeSlot = slot;
        
        buyButton.interactable = !isPlayersSlot && player.Money.GetMoney() >= slot.Item.Cost;
        sellButton.interactable = isPlayersSlot;
    }
    public void ResetButtons()
    {
        buyButton.interactable = false;
        sellButton.interactable = false;
    }
}
