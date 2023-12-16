using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class FastTravelUI : InGameUiWindow
{
    [SerializeField] private string sceneName;
    [SerializeField] private List<Button> buttons;
    [SerializeField] private Button hubButton;
    
    protected override void Awake()
    {
        base.Awake();
        var hubTransition = hubButton.GetComponent<LevelTransition>();
        hubTransition.SetTransitionData(player);
        hubButton.onClick.AddListener(hubTransition.LoadLevel);
        
        for (int i = 0; i < buttons.Count; i++)
        {
            var levelTransition = buttons[i].GetComponent<LevelTransition>();
            levelTransition.SetTransitionData(player, $"{sceneName}{i + 1}");
            buttons[i].onClick.AddListener(levelTransition.LoadLevel);
            buttons[i].interactable = i < LevelTransition.MaxReachedLevel && SceneManager.GetActiveScene().buildIndex != i + 1;
        }
    }
}
