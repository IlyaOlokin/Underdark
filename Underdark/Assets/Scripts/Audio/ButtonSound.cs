using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [SerializeField] private string soundName = "ButtonClick1";
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(DoSound);
    }

    private void DoSound()
    {
        AudioManager.Instance.Play(soundName);
    }
}
