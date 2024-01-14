using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float appearDuration;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            StopAllCoroutines();
            StartCoroutine(AnimateText(true));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            StopAllCoroutines();
            if (gameObject.activeInHierarchy) StartCoroutine(AnimateText(false));
        }
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    private IEnumerator AnimateText(bool appear)
    {
        if (appear) text.gameObject.SetActive(true);
        
        float timer = 0f; // Таймер для отслеживания прогресса анимации

        while (timer < appearDuration)
        {
            timer += Time.deltaTime; // Увеличиваем таймер на время, прошедшее с последнего кадра

            // Вычисляем текущий прогресс анимации
            float progress;
            if (appear) progress = Mathf.Clamp01(timer / appearDuration);
            else progress = 1f - Mathf.Clamp01(timer / appearDuration);


            // Устанавливаем новое значение альфа-компонента цвета текста
            Color newColor = text.color;
            newColor.a = progress;
            text.color = newColor;

            yield return null; // Ждем следующего кадра
        }
        
        if (!appear) text.gameObject.SetActive(false);
    }
}
