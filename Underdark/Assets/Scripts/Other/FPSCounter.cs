using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private int frameCount = 10;

    private float[] frameTimes;
    private int currentIndex;

    private void Start()
    {
        frameTimes = new float[frameCount];
    }

    private void Update()
    {
        float frameTime = Time.unscaledDeltaTime;
        frameTimes[currentIndex] = frameTime;
        currentIndex = (currentIndex + 1) % frameCount;

        float totalFrameTime = 0f;
        for (int i = 0; i < frameCount; i++)
        {
            totalFrameTime += frameTimes[i];
        }

        float averageFrameTime = totalFrameTime / frameCount;
        float fps = 1f / averageFrameTime;
        fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
    }
}