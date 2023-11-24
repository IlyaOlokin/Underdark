using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StaticSceneLoader
{
    public static string SceneToLoadName;
    public static bool ResetPlayer;
    
    public static void LoadScene(int sceneID, bool resetPlayer = false)
    {
        SceneToLoadName = SceneNameFromIndex(sceneID);
        ResetPlayer = resetPlayer;
        SceneManager.LoadScene("LoadScene");
    }
    
    public static void LoadScene(string sceneName, bool resetPlayer = false)
    {
        SceneToLoadName = sceneName;
        ResetPlayer = resetPlayer;
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
