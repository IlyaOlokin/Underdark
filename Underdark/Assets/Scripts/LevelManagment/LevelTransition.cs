using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelTransition : MonoBehaviour
{
    public static int MaxReachedLevel;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            this.player = player;
            OnLoad?.Invoke();
            LoadLevel();
        }
    }

    public void SetTransitionData(Player player, string name = "")
    {
        this.player = player;
        if (!name.Equals("")) sceneName = name;
    }

    public void LoadLevel()
    {
        if (loadSceneMode == LoadMode.Next && SceneManager.GetActiveScene().buildIndex + 1 > MaxReachedLevel)
            MaxReachedLevel = SceneManager.GetActiveScene().buildIndex + 1;
        
        DataLoader.SaveGame(player);

        SaveElixirCd(player);
        
        switch (loadSceneMode)
        {
            case LoadMode.Next:
                StartFromUp = true;
                StaticSceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case LoadMode.Previous:
                StartFromUp = false;
                StaticSceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                break;
            case LoadMode.Custom:
                StartFromUp = true;
                StaticSceneLoader.LoadScene(sceneName);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
