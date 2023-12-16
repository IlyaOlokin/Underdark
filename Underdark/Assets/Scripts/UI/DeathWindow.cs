using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeathWindow : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject content;
    [SerializeField] private Button respawnButton;
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        respawnButton.onClick.AddListener(LoadHub);
    }

    private void OnEnable()
    {
        player.OnUnitDeath += ActivateContent;
    }
    
    private void OnDisable()
    {
        player.OnUnitDeath -= ActivateContent;
    }

    private void ActivateContent()
    {
        transform.SetAsLastSibling();
        content.SetActive(true);
    }

    private void LoadHub()
    {
        StaticSceneLoader.LoadScene("Hub", true);
    }
}
