using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Item containedItem;
    [SerializeField] private int itemAmount = 1;
    [SerializeField] private float speed;
    [SerializeField] private float timeToIntractable;
    private bool picked;
    private Transform target;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        //SetDroppedItem(containedItem, itemAmount);
    }

    private void Update()
    {
        timeToIntractable -= Time.deltaTime;
        if (timeToIntractable <= 0) collider.enabled = true;
        if (!picked) return;

        if (Vector3.Distance(transform.position, target.position) <= 0.1f) Destroy(gameObject);
        else transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    public void SetDroppedItem(Item item, int amount)
    {
        containedItem = item;
        itemAmount = amount;
        sr.sprite = item.Sprite;
        rb.AddForce(new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f)) * 3, ForceMode2D.Impulse);
        
        collider.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!picked && other.TryGetComponent(out IPickUper pickUper))
        {
            if (pickUper.TryPickUpItem(containedItem, itemAmount))
            {
                picked = true;
                target = other.transform;
            }
        }
    }
}
