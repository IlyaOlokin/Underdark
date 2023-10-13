using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackVisual : MonoBehaviour
{
    [SerializeField] private float swingDuration = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform swingPoint;
    private float swingTimer;

    public void Swing(float dir, float angle, float dist)
    {
        StartCoroutine(StartSwing(dir, angle, dist));
        Destroy(gameObject, 0.3f);
    }

    private IEnumerator StartSwing(float dir, float angle, float dist)
    {
        var extraAngle = angle * 0.2f;
        transform.eulerAngles = new Vector3(0, 0, dir - angle / 2f - extraAngle);
        swingPoint.localPosition = new Vector3(dist / 2f, 0);
        trailRenderer.widthMultiplier *= dist;
        swingTimer = swingDuration;
        float rotationSpeed = (angle + extraAngle) / swingDuration;
        while (swingTimer > 0)
        {
            transform.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed, Space.World);
            swingTimer -= Time.deltaTime;
            yield return null;
        }
    }
}