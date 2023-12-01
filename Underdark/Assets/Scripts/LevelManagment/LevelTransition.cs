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
    
    [SerializeField] private LoadMode loadSceneMode;
    [SerializeField] private string sceneName;
    private Player player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            this.player = player;
            LoadLevel();
        }
    }

    public void SetScene(Player player, string name)
    {
        this.player = player;
        sceneName = name;
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
                StaticSceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case LoadMode.Previous:
                StaticSceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                break;
            case LoadMode.Custom:
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
