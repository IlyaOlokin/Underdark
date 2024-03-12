using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class FastTravelUI : InGameUiWindow
{
    [SerializeField] private LevelConfigSO levelConfig;

    [SerializeField] private Button hubButton;

    [SerializeField] private string sceneName;
    [SerializeField] private FastTravelUICell fastTravelUICellPref;
    [SerializeField] private Transform fastTravelCellsParent;

    
    
    protected override void Awake()
    {
        base.Awake();
        
        SetHubButton();
        SetLevelButtons();
    }

    private void SetLevelButtons()
    {
        for (int i = 0; i < levelConfig.Floors.Count; i++)
        {
            var interactable = i < LevelTransition.MaxReachedFloor;
            
            var newFastTravelUICell = Instantiate(fastTravelUICellPref, fastTravelCellsParent);
            newFastTravelUICell.Init(i, levelConfig, interactable, player, sceneName);
        }
    }

    private void SetHubButton()
    {
        var hubTransition = hubButton.GetComponent<LevelTransition>();
        hubTransition.SetTransitionData(player);
        hubButton.onClick.AddListener(hubTransition.LoadLevel);
    }
}
