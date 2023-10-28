using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Item containedItem;
    [SerializeField] private int itemAmount = 1;
    [SerializeField] private float speed;
    [SerializeField] private float timeToIntractable;
    private bool picked;
    private Transform target;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Collider2D coll;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        //SetDroppedItem(containedItem, itemAmount);
    }

    private void Update()
    {
        timeToIntractable -= Time.deltaTime;
        if (timeToIntractable <= 0) coll.enabled = true;
        if (!picked) return;

        if (Vector3.Distance(transform.position, target.position) <= 0.1f)
        {
            picked = false;
            if (itemAmount == 0)
                Destroy(gameObject);
        }
        else transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    public void SetDroppedItem(Item item, int amount)
    {
        containedItem = item;
        itemAmount = amount;
        sr.sprite = item.Sprite;
        text.text = itemAmount.ToString();
        rb.AddForce(new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f)) * 3, ForceMode2D.Impulse);
        
        coll.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!picked && other.TryGetComponent(out IPickUper pickUper))
        {
            var itemsLeft = pickUper.TryPickUpItem(containedItem, itemAmount);
            if (itemsLeft != itemAmount)
            {
                itemAmount = itemsLeft;
                text.text = itemAmount.ToString();
                picked = true;
                target = other.transform;
            }
        }
    }
}
