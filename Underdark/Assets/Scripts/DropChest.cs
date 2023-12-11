using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropChest : MonoBehaviour
{
    [SerializeField] protected PlayerSensor playerSensor;
    [SerializeField] protected Drop drop;
    private bool opened;

    [Header("Visual")]
    [SerializeField] protected Sprite openedSprite;
    [SerializeField] protected SpriteRenderer sr;

    void Start()
    {
        playerSensor.OnPlayerEnter += (player) =>
        {
            if (!opened) drop.DropItems(player.GetComponent<IMoneyHolder>());
            opened = true;
            sr.sprite = openedSprite;
        };
    }
}
