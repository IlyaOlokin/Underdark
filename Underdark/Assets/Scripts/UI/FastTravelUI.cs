using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class FastTravelUI : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private List<Button> buttons;
    [SerializeField] private Button hubButton;
    [SerializeField] private Button closeButton;
    private Player player;
    private IInput input;

    [Inject]
    private void Construct(Player player, IInput input)
    {
        this.player = player;
        this.input = input;
    }
    
    private void Awake()
    {
        var hubTransition = hubButton.GetComponent<LevelTransition>();
        hubTransition.SetTransitionData(player);
        hubButton.onClick.AddListener(hubTransition.LoadLevel);
        
        for (int i = 0; i < buttons.Count; i++)
        {
            var levelTransition = buttons[i].GetComponent<LevelTransition>();
            levelTransition.SetTransitionData(player, $"{sceneName}{i + 1}");
            buttons[i].onClick.AddListener(levelTransition.LoadLevel);
            buttons[i].interactable = i < LevelTransition.MaxReachedLevel;
        }
        
        closeButton.onClick.AddListener(CloseWindow);
    }

    private void CloseWindow()
    {
        input.IsEnabled = true;
        gameObject.SetActive(false);
    }
    
    public void OpenWindow()
    {
        input.IsEnabled = false;
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }
}
