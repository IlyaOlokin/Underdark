using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class NotificationPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float lifeTime;
    private RectTransform rt;
    private float lifeTimer;
    private bool isNotificationActive;

    private void Awake()
    {
        rt = image.transform.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isNotificationActive)
        {
            lifeTimer += Time.unscaledDeltaTime;
            if (lifeTimer >= lifeTime)
            {
                HideNotification();
            }
        }
    }

    public void ShowNotification(Notification notification)
    {
        float aspectRatio = 0;
        if (notification.sprite != null)
        {
            Rect spriteRect = notification.sprite.rect;
            aspectRatio = spriteRect.width / spriteRect.height;
        }

        Vector2 newSizeDelta;
        if (aspectRatio > 1)
            newSizeDelta = new Vector2(75f, 75 / aspectRatio);
        else
            newSizeDelta = new Vector2(75 * aspectRatio, 75f);
        
        rt.sizeDelta = newSizeDelta;
        image.sprite = notification.sprite;
        text.text = notification.text;
        lifeTimer = 0;
        
        OnNotificationActivate();
        transform.DOMoveY(86, 0.75f).SetEase(Ease.OutBack);
        
        isNotificationActive = true;
    }
    private void HideNotification()
    {
        transform.DOMoveY(-106f, 0.75f).SetEase(Ease.InBack).SetDelay(lifeTime).OnComplete(OnNotificationDeactivate);
        isNotificationActive = false;
    }

    private void OnNotificationActivate()
    {
        NotificationManager.Instance.OnNotificationActivate();
    }
    
    private void OnNotificationDeactivate()
    {
        NotificationManager.Instance.OnNotificationDeactivate();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        HideNotification();
    }
}
