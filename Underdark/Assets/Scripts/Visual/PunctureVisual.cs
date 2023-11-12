using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunctureVisual : MonoBehaviour
{
    [SerializeField] private float visualDuration;
    [SerializeField] private float endXScale;
    [SerializeField] private float startYScale;

    public void StartVisualEffect(Transform target)
    {
        Destroy(gameObject, visualDuration);
        StartCoroutine(StartVisual(target.transform));
    }
    
    IEnumerator StartVisual(Transform target)
    {
        transform.gameObject.SetActive(true);
        transform.position = target.position;
        transform.localScale = new Vector3(0, startYScale);
        float scaleXSpeed =  endXScale / visualDuration;
        float scaleYSpeed = -transform.localScale.y / visualDuration;
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        
        while (visualDuration > 0)
        {
            visualDuration -= Time.deltaTime;
            transform.localScale += new Vector3(scaleXSpeed * Time.deltaTime, scaleYSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
