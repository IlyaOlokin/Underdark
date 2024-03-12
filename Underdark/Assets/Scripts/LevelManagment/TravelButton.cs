using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TravelButton : MonoBehaviour
{
    [SerializeField] private LevelTransition levelTransition;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonLabel;
    
    public void Init(bool interactable, Player player, string sceneName, string sceneSuffix)
    {
        button.interactable = interactable;
        
        if (LevelTransition.GetCurrentLevel().Equals(sceneSuffix))
            button.interactable = false;
        
        buttonLabel.text = sceneSuffix;
        if (!interactable) return;
        
        levelTransition.SetTransitionData(player, $"{sceneName}{sceneSuffix}");
        
        button.onClick.AddListener(levelTransition.LoadLevel);
        
    }
}
