using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropChest : MonoBehaviour
{
    [SerializeField] private PlayerSensor playerSensor;
    [SerializeField] private Drop drop;
    private bool opened;

    [Header("Visual")]
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private InGameToolTip inGameToolTip;

    private void Awake()
    {
        inGameToolTip.SetText($"Chest: {OpenState()}");
    }

    void Start()
    {
        playerSensor.OnPlayerEnter += (player) =>
        {
            if (!opened) drop.DropItems(player.GetComponent<IMoneyHolder>());
            opened = true;
            sr.sprite = openedSprite;
            inGameToolTip.SetText($"Chest: {OpenState()}");
        };
    }

    private string OpenState()
    {
        return opened ? "Opened" : "Closed";
    }
}
