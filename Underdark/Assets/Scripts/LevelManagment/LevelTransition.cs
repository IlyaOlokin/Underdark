using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelTransition : MonoBehaviour
{
    public static int MaxReachedFloor;
    public static bool TutorialCompleted;
    public static bool StartFromUp = true;

    public event Action OnLoad; 
    
    [SerializeField] private LoadMode loadSceneMode;
    [SerializeField] private string sceneName;
    private Player player;

    [SerializeField] private InGameToolTip inGameToolTip;

    private void Awake()
    {
        if (inGameToolTip == null) return;
        switch (loadSceneMode)
        {
            case LoadMode.Next:
                inGameToolTip.SetText($"Next Level");
                break;
            case LoadMode.Previous:
                inGameToolTip.SetText("Previous Level");
                break;
            case LoadMode.Custom:
                inGameToolTip.SetText(sceneName);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void SetTransitionData(Player player, string name = "")
    {
        this.player = player;
        if (!name.Equals("")) sceneName = name;
    }

    public void LoadLevel()
    {
        OnLoad?.Invoke();
        
        DataLoader.SaveGame(player);

        SaveElixirCd(player);
        
        StaticSceneLoader.LoadScene(sceneName);
    }

    public static string GetCurrentLevel()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
        if (!currentSceneName.Contains("Level")) return "-1";
        return currentSceneName.Substring(5);
    }
    
    public static int GetCurrentFloorIndex()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
        if (!currentSceneName.Contains("Level")) return -1;
        return int.Parse(currentSceneName.Substring(5, 1)) - 1;
    }

    private static void SaveElixirCd(Component player)
    {
        if (player.TryGetComponent(out Elixir elixir))
        {
            ElixirStaticData.ElixirCD = elixir.Timer;
        }
    }
}

public enum LoadMode
{
    Next,
    Previous,
    Custom
}
