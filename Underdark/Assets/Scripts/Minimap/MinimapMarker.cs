using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class MinimapMarker : MonoBehaviour
{
    [SerializeField] private Texture2D maskTexture;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color visitedColor;
    [SerializeField] private float radius;
    [SerializeField] private float mapHalfSize;
    
    private void Awake()
    {
        Color[] colors = maskTexture.GetPixels();
        for (int x = 0; x < maskTexture.width; x++)
        {
            for (int y = 0; y < maskTexture.height;y++)
            {
                int index = y * maskTexture.width + x;
                colors[index] = defaultColor;
            }
        }
        maskTexture.SetPixels(colors);
        maskTexture.Apply();

        StartCoroutine(UpdateMaskTexture());
    }
    
    private IEnumerator UpdateMaskTexture()
    {
        while (true)
        {
            Vector3 playerPosition = transform.position;
            Color[] colors = maskTexture.GetPixels();
            yield return null;

            playerPosition.x = (playerPosition.x + mapHalfSize) / (mapHalfSize * 2f) * maskTexture.width;
            playerPosition.y = (playerPosition.y + mapHalfSize) / (mapHalfSize * 2f) * maskTexture.width;
            
            for (int x = (int)(playerPosition.x - radius); x <= playerPosition.x + radius; x++)
            {
                if (x < 0 || x >= maskTexture.width) continue;
                for (int y = (int)(playerPosition.y - radius); y <= playerPosition.y + radius; y++)
                {
                    if (y < 0 ||  y >= maskTexture.height) continue;
                    if (Vector2.Distance(playerPosition, new Vector2(x, y)) <= radius)
                    {
                        int index = y * maskTexture.width + x;
                        if (index >= 0 && index < colors.Length)
                            colors[index] = visitedColor;
                    }
                }
            }

            maskTexture.SetPixels(colors);
            maskTexture.Apply();
            yield return null;
        }
    }
}
