using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StaticSceneLoader
{
    public static string sceneToLoadName;
    
    public static void LoadScene(int sceneID)
    {
        sceneToLoadName = SceneNameFromIndex(sceneID);
        SceneManager.LoadScene("LoadScene");
    }
    
    public static void LoadScene(string sceneName)
    {
        sceneToLoadName = sceneName;
        SceneManager.LoadScene("LoadScene");
    }
    
    private static string SceneNameFromIndex(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
}
