using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : InGameUiWindow
{
    [SerializeField] private Button mainMenuButton;
    private const string MainMenuName = "MainMenu";
    
    protected override void Awake()
    {
        base.Awake();
        mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        DataLoader.SaveGame(player);
        StaticSceneLoader.LoadScene(MainMenuName);
    }
    
    protected override void CloseWindow()
    {
        base.CloseWindow();
        Time.timeScale = 1f;
    }
    
    public override void OpenWindow()
    {
        base.OpenWindow();
        Time.timeScale = 0f;
    }
}
