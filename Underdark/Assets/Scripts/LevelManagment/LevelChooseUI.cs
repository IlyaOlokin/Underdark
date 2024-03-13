using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChooseUI : InGameUiWindow
{
    [SerializeField] private LevelConfigSO levelConfig;
    
    [SerializeField] private string sceneName;
    [SerializeField] private FastTravelUICell fastTravelUICell;

    public void Init(int targetFloorIndex)
    {
        var interactable = targetFloorIndex < LevelTransition.MaxReachedFloor + 2;
        fastTravelUICell.Init(targetFloorIndex, levelConfig, interactable, player, sceneName);
    }
}
