using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSourceVisual : MonoBehaviour
{
    [SerializeField] private Light2D light2d;

    [SerializeField] private float lightUpTime;

    private float intensity;
    private float radius;

    public void LightUp()
    {
        intensity = light2d.intensity;
        radius = light2d.pointLightOuterRadius;
        StartCoroutine(TransformLight(true));
    }
    
    public void LightDown()
    {
        transform.SetParent(null);
        StartCoroutine(TransformLight(false));
    }

    private IEnumerator TransformLight(bool lightUp)
    {
        var targetIntensity = lightUp ? intensity : 0;
        var targetRadius = lightUp ? radius : 0;
        
        var startIntensity = lightUp ? 0 : intensity;
        var startRadius = lightUp ? 0 : radius;

        light2d.intensity = startIntensity;
        light2d.pointLightOuterRadius = startRadius;

        var progress = 0f;
        while (progress < lightUpTime)
        {
            progress += Time.deltaTime;
            light2d.intensity = Mathf.Lerp(startIntensity, targetIntensity, progress / lightUpTime);
            light2d.pointLightOuterRadius = Mathf.Lerp(startRadius, targetRadius, progress / lightUpTime);

            yield return null;
        }

        light2d.intensity = targetIntensity;
        light2d.pointLightOuterRadius = targetRadius;
        
        if (!lightUp) Destroy(gameObject);
    }
}
