using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityLevelDisplayUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI progressText;

    public void DisplayAbilityLevel(ActiveAbilitySO abilitySO, int exp)
    {
        icon.sprite = abilitySO.Sprite;
        UpdateProgress(abilitySO, exp);
    }

    private void UpdateProgress(ActiveAbilitySO abilitySO, int exp)
    {
        var level = abilitySO.ActiveAbility.ActiveAbilityLevelSetupSO.GetCurrentLevel(exp);
        var progress = abilitySO.ActiveAbility.ActiveAbilityLevelSetupSO.GetCurrentProgressInPercent(exp);

        currentLevel.text = $"Level {RomanConverter.NumberToRoman(level)}";
        progressBar.value = progress;
        
        if (level > 0)
            progressText.text = progress < 1 ? Mathf.Floor(progress * 100) + "%" : "Max!";
        else
            progressText.text = "";
    }
}
