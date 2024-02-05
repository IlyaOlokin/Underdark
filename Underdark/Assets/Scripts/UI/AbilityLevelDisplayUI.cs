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
        currentLevel.text = (abilitySO.ActiveAbility.ActiveAbilityLevelSetupSo.GetCurrentLevel(exp)).ToString();
        var progress = abilitySO.ActiveAbility.ActiveAbilityLevelSetupSo.GetCurrentProgressInPercent(exp);

        progressText.text = progress < 1 ? Mathf.Floor(progress * 100) + "%" : "Max Level";
        progressBar.value = progress;
    }
}
