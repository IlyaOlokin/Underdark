using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    private event Action OnNotificationQueueChanged;

    private Queue<Notification> notificationQueue = new Queue<Notification>();
    private bool isNotificationActive;
    [SerializeField] private NotificationPanel notificationPanel;
    [SerializeField] private Sprite test;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        OnNotificationQueueChanged += TryShowNotification;
    }
    private void OnDisable()
    {
        OnNotificationQueueChanged -= TryShowNotification;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SendNotification(new Notification(test, "подсказка работает? " + notificationQueue.Count));
        }
    }

    public void SendNotification(Notification notification)
    {
        notificationQueue.Enqueue(notification);
        OnNotificationQueueChanged?.Invoke();
    }

    private void TryShowNotification()
    {
        if (isNotificationActive) return;
        if (notificationQueue.Count > 0)
            notificationPanel.ShowNotification(notificationQueue.Dequeue());
    }

    public void OnNotificationActivate()
    {
        isNotificationActive = true;
    }

    public void OnNotificationDeactivate()
    {
        isNotificationActive = false;
        OnNotificationQueueChanged?.Invoke();
    }
}