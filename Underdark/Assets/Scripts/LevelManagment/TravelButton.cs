using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TravelButton : MonoBehaviour
{
    [SerializeField] private LevelTransition levelTransition;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonLabel;
    [SerializeField] private GameObject activeIndicator;
    [SerializeField] private Transform elementalIconStartPos;
    [SerializeField] private ElementalIconUI elementalIconPref;
    [SerializeField] private float xOffset;
    
    public void Init(bool interactable, Player player, string sceneName, string sceneSuffix, List<DamageType> damageTypes)
    {
        button.interactable = interactable;
        activeIndicator.SetActive(false);

        if (LevelTransition.GetCurrentLevel().Equals(sceneSuffix))
        {
            button.interactable = false;
            activeIndicator.SetActive(true);
        }
        
        for (var i = 0; i < damageTypes.Count; i++)
        {
            var newIcon = Instantiate(elementalIconPref, elementalIconStartPos.position, Quaternion.identity, elementalIconStartPos);
            newIcon.transform.localPosition = new Vector3(i * xOffset, 0);
            newIcon.Init(damageTypes[i]);
        }
        
        buttonLabel.text = sceneSuffix;
        if (!interactable) return;
        
        levelTransition.SetTransitionData(player, $"{sceneName}{sceneSuffix}");
        
        button.onClick.AddListener(levelTransition.LoadLevel);
    }
}
