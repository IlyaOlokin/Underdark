using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Slider bar;
    private Player player;
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        this.player.Stats.OnExpChanged += UpdateBar;
    }

    private void Awake()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (player.Stats.ExpToNextLevel() == -1)
        {
            bar.value = 1;
            return;
        }
        bar.value = player.Stats.CurrentExp / (float) player.Stats.ExpToNextLevel();
    }
}
