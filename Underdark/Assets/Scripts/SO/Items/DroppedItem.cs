using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DroppedItem : MonoBehaviour
{
    private Item containedItem;
    private int itemAmount = 1;
    [SerializeField] private float speed;
    [SerializeField] private float timeToInteractable;
    [SerializeField] private float timeToInteractableAfterPlayerDrop;
    private bool picked;
    private bool readyToPickUpAfterPlayerDrop;
    private Transform target;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Collider2D coll;
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        if (!picked) return;

        if (Vector3.Distance(transform.position, target.position) <= 0.5f)
        {
            picked = false;
            if (itemAmount == 0)
                Destroy(gameObject);
        }
        else transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    public void SetDroppedItem(Item item, int amount, int force = 4, bool droppedByPlayer = false)
    {
        containedItem = item;
        itemAmount = amount;
        sr.sprite = item.Sprite;
        text.text = itemAmount == 0 ? "" : itemAmount.ToString();
        readyToPickUpAfterPlayerDrop = !droppedByPlayer;
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * force, ForceMode2D.Impulse);

        StartCoroutine(InteractDelay());
    }

    IEnumerator InteractDelay()
    {
        coll.enabled = false;
        yield return new WaitForSeconds(timeToInteractable);
        coll.enabled = true;
        yield return new WaitForSeconds(timeToInteractableAfterPlayerDrop);
        readyToPickUpAfterPlayerDrop = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!picked && readyToPickUpAfterPlayerDrop && other.TryGetComponent(out IPickUper pickUper))
        {
            var itemsLeft = pickUper.TryPickUpItem(containedItem, itemAmount);
            if (itemsLeft != itemAmount)
            {
                itemAmount = itemsLeft;
                text.text = itemAmount == 0 ? "" : itemAmount.ToString();
                picked = true;
                target = other.transform;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            readyToPickUpAfterPlayerDrop = true;
        }
    }
}
