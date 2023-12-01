using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private LoadMode loadSceneMode;
    [SerializeField] private string sceneName;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
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
    }

    private static void SaveElixirCd(Component player)
    {
        if (player.TryGetComponent(out Elixir elixir))
        {
            ElixirStaticData.ElixirCD = elixir.Timer;
        }
    }
}

internal enum LoadMode
{
    Next,
    Previous,
    Custom
}
