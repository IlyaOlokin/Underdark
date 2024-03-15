using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Door : MonoBehaviour
{
    [SerializeField] private LevelConfigSO levelConfig;
    [SerializeField] private LevelTransition levelTransition;
    [SerializeField] private LoadMode loadSceneMode;

    private LevelChooseUI levelChooseUI;


    [Inject]
    private void Construct(LevelChooseUI levelChooseUI)
    {
        this.levelChooseUI = levelChooseUI;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (loadSceneMode == LoadMode.Next)
            {
                LevelTransition.StartFromUp = true;
                levelChooseUI.OpenWindow();
                levelChooseUI.Init(LevelTransition.GetCurrentFloorIndex() + 1);
            }
            else if (loadSceneMode == LoadMode.Previous)
            {
                LevelTransition.StartFromUp = false;
                levelChooseUI.OpenWindow();
                levelChooseUI.Init(LevelTransition.GetCurrentFloorIndex() - 1);
            }
            else
            {
                LevelTransition.StartFromUp = true;
                levelTransition.SetTransitionData(player);
                levelTransition.LoadLevel();
            }
        }
    }
}
