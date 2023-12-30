using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void PlayButton(string sceneName)
    {
        StaticSceneLoader.LoadScene(LevelTransition.TutorialCompleted ? "Hub" : "Tutorial", true);
    }
    
    public void LoadScene(string sceneName)
    {
        StaticSceneLoader.LoadScene(sceneName, true);
    }

    public void ClearSaves()
    {
        DataLoader.NewGame();
    }
    

    public void Quit()
    {
        Application.Quit();
    }
}
