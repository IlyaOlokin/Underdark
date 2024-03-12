using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TravelButton : MonoBehaviour
{
    [SerializeField] private LevelTransition levelTransition;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonLabel;
    
    public void Init(bool interactable, Player player, string sceneName, string sceneSuffix)
    {
        button.interactable = interactable;
        buttonLabel.text = sceneSuffix;
        if (!interactable) return;
        
        levelTransition.SetTransitionData(player, $"{sceneName}{sceneSuffix}");
        
        button.onClick.AddListener(levelTransition.LoadLevel);
        
    }
}
