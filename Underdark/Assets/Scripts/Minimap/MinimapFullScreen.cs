using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MinimapFullScreen : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject blackOut;
    [SerializeField] private float fullScreenScaleMultiplier;
    private Vector3 startScale;
    private Vector3 startPos;
    private bool isFullScreen;
    void Start()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        if (isFullScreen && Input.anyKeyDown)
        {
            CloseFullScreen();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OpenFullScreen();
    }

    private void OpenFullScreen()
    {
        StartCoroutine(SetIsFullScreenWithDelay(true));
        blackOut.SetActive(true);
        transform.localScale *= fullScreenScaleMultiplier;
        transform.localPosition = blackOut.transform.localPosition;
    }

    private void CloseFullScreen()
    {
        isFullScreen = false;
        blackOut.SetActive(false);
        transform.localScale = startScale;
        transform.localPosition = startPos;
    }

    private IEnumerator SetIsFullScreenWithDelay(bool isFullScreen)
    {
        yield return null;
        this.isFullScreen = isFullScreen;
    }
}
