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
            if (loadNextScene)
                StaticSceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                StaticSceneLoader.LoadScene(sceneName);
        }
    }
}
