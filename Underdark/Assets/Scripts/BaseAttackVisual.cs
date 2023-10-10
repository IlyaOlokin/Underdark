using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackVisual : MonoBehaviour
{
    [SerializeField] private float swingDuration = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    private float swingTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(Swing(Vector3.up, 90));
    }

    private IEnumerator Swing(Vector3 dir, float angle)
    {
        trailRenderer.enabled = false;
        transform.eulerAngles = new Vector3(dir.x, dir.y, dir.z - angle / 2f);
        trailRenderer.enabled = true;
        swingTimer = swingDuration;
        float rotationSpeed = angle / swingDuration;
        while (swingTimer > 0)
        {
            transform.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed, Space.World);
            swingTimer -= Time.deltaTime;
            yield return null;
        }
    }
}
