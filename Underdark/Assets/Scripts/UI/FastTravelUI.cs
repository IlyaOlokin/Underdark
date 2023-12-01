using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FastTravelUI : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private List<Button> buttons;
    private Player player;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }
    
    private void Awake()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var levelTransition = buttons[i].GetComponent<LevelTransition>();
            levelTransition.SetScene(player, $"{sceneName}{i + 1}");
            buttons[i].onClick.AddListener(levelTransition.LoadLevel);
        }
    }
}
