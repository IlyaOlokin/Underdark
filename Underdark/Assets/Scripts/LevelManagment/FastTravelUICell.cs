using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FastTravelUICell : MonoBehaviour
{
    [SerializeField] private TravelButton travelButtonPref;

    public void Init(int floorNumber, LevelConfigSO levelConfig, bool interactable, Player player, string sceneName)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        
        var buttonsCount = levelConfig.Floors[floorNumber].LevelsCount;
        var levelNumber = 1;
        
        for (int i = -(buttonsCount - 1) / 2 ; i < buttonsCount / 2 + 1; i++)
        {
            var newButton = Instantiate(travelButtonPref, transform);
            var levelSuffix = buttonsCount > 1 ? $"{floorNumber + 1}.{levelNumber}" : (floorNumber + 1).ToString();
            newButton.Init(interactable, player, sceneName, levelSuffix, levelConfig.Floors[floorNumber].DamageTypesLists[levelNumber - 1].DamageTypes );
            levelNumber++;
        }
    }
}
