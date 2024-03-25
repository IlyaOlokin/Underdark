using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Image image;

    private void Start()
    {
        ChangeVisual(DataLoader.MetaGameData.MusicOn);
    }

    public void ToggleMusic()
    {
        ChangeVisual(AudioManager.Instance.ToggleMusic());
    }

    private void ChangeVisual(bool needOnSprite)
    {
        image.sprite = needOnSprite ? onSprite : offSprite;
    }
}
