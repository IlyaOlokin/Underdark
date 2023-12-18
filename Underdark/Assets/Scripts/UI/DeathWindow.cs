using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeathWindow : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject content;
    [SerializeField] private Button respawnButton;

    [Header("Visual")] 
    [SerializeField] private float appearDuration;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image blackout;
    [SerializeField] private Image buttonImage;
        
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        respawnButton.onClick.AddListener(LoadHub);
    }

    private void OnEnable()
    {
        player.OnUnitDeath += StartActivateContent;
    }
    
    private void OnDisable()
    {
        player.OnUnitDeath -= StartActivateContent;
    }

    private void StartActivateContent()
    {
        StartCoroutine(ActivateContent());
    }

    IEnumerator ActivateContent()
    {
        transform.SetAsLastSibling();
        content.SetActive(true);

        float timer = 0f; 

        while (timer < appearDuration)
        {
            timer += Time.deltaTime; 
            
            float progress = Mathf.Clamp01(timer / appearDuration);
            
            ChangeColorAlpha(text, progress);
            ChangeColorAlpha(blackout, progress);
            ChangeColorAlpha(buttonImage, progress);

            yield return null;
        }
    }

    private void ChangeColorAlpha(Image image, float a)
    {
        Color newColor = image.color;
        newColor.a = a;
        image.color = newColor;
    }
    
    private void ChangeColorAlpha(TextMeshProUGUI image, float a)
    {
        Color newColor = image.color;
        newColor.a = a;
        image.color = newColor;
    }
    

    private void LoadHub()
    {
        StaticSceneLoader.LoadScene("Hub", true);
    }
}
