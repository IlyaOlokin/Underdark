using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    public static InputType InputType;
    
    public  void Awake()
    {
#if UNITY_ANDROID
        InputType = InputType.Mobile;
        Application.targetFrameRate = 60;
#endif
        
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        InputType = InputType.Mobile;
        Application.targetFrameRate = -1;
#endif
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}