using System.Collections;
using UnityEngine;

public class FireCircleVisual : MonoBehaviour
{
    [SerializeField] private Transform particles;

    private void Start()
    {
        StartVisualEffect(5f, 7f);
    }

    public void StartVisualEffect(float duration, float radius)
    {
        var endScale = radius * 2 + 1;
        var scaleSpeed = endScale / duration;
        
        Destroy(gameObject, duration);
        StartCoroutine(StartVisual(scaleSpeed));
    }
    
    IEnumerator StartVisual(float scaleSpeed)
    {
        while (true)
        {
            var newScale = particles.localScale.x + scaleSpeed * Time.deltaTime;
            particles.localScale = new Vector3(newScale, newScale);
            transform.localScale = new Vector3(newScale, newScale);
            
            yield return null;
        }
    }
}
