using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Image image;

    private void Start()
    {
        ChangeVisual(DataLoader.MetaGameData.SoundOn);
    }

    public void ToggleSound()
    {
        ChangeVisual(AudioManager.Instance.ToggleSound());
    }

    private void ChangeVisual(bool needOnSprite)
    {
        image.sprite = needOnSprite ? onSprite : offSprite;
    }
}
