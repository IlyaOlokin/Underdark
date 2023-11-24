using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    
    
    [SerializeField] private bool loadNextScene;
    [SerializeField] private string sceneName;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            DataLoader.SaveGame(player);

            SaveElixirCd(player);
            
            if (loadNextScene)
                StaticSceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                StaticSceneLoader.LoadScene(sceneName);
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
