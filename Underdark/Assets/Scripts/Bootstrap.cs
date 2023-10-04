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
#endif
        
#if UNITY_EDITOR
        InputType = InputType.Mobile;
#endif
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}