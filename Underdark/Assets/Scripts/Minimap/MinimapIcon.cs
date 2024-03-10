using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField] private float distToAppear;
    [SerializeField] private SpriteRenderer sr;
    private float searchDelay = 0.05f;
    private Player player;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }

    private void Awake()
    {
        sr.enabled = false;
        StartCoroutine(TryShowIcon());
    }

    private IEnumerator TryShowIcon()
    {
        while (true)
        {
            if (Vector2.Distance(player.transform.position, transform.position) < distToAppear)
            {
                sr.enabled = true;
                yield break;
            }

            yield return new WaitForSeconds(searchDelay);
        }
    }
}
