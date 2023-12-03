using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunctureVisual : MonoBehaviour
{
    [SerializeField] private float visualDuration;
    [SerializeField] private float endXScale;
    [SerializeField] private float startYScale;

    public void StartVisualEffect(Vector3 pos)
    {
        Destroy(gameObject, visualDuration);
        StartCoroutine(StartVisual(pos));
    }
    
    IEnumerator StartVisual(Vector3 pos)
    {
        transform.gameObject.SetActive(true);
        transform.position = pos;
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
