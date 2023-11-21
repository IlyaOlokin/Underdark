using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Slider slider;
    void Start()
    {
        StartCoroutine(AsyncLoadScene(StaticSceneLoader.sceneToLoadName));
    }

    
    IEnumerator AsyncLoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            text.text = $"Loading... {progress * 100}%";
            
            yield return null;
        }
    }
}
