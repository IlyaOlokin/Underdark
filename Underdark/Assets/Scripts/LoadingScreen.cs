using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Slider slider;

    private static bool dataLoaded;
    void Start()
    {
        StartCoroutine(AsyncLoadScene(StaticSceneLoader.SceneToLoadName));
        ClientSend.LoadReceived(DataLoader.metaGameData.Email);
    }
    
    IEnumerator AsyncLoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone || !dataLoaded)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            text.text = $"Loading... {progress * 100}%";
            
            yield return null;
        }
    }

    public static void LoadDataReceived(string data)
    {
        dataLoaded = true;
        DataLoader.LoadData(data);
    }
}
