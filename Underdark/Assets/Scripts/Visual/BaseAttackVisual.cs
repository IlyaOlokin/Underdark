using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackVisual : MonoBehaviour
{
    [SerializeField] private float swingDuration = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform swingPoint;
    private float swingTimer;

    public void Swing(float dir, float angle, float dist, bool reversed)
    {
        StartCoroutine(StartSwing(dir, angle, dist + 1, reversed));
        Destroy(gameObject, swingDuration);
    }

    private IEnumerator StartSwing(float dir, float angle, float dist, bool reversed)
    {
        var extraAngle = angle * 0.2f;
        transform.eulerAngles = new Vector3(0, 0, dir + (reversed ? -(angle / 2f + extraAngle) :  angle / 2f + extraAngle));
        swingPoint.localPosition = new Vector3(dist / 1.5f, 0);
        trailRenderer.widthMultiplier *= dist;
        swingTimer = swingDuration;
        float rotationSpeed = (angle + extraAngle) / swingDuration * (reversed ? 1 : -1);
        while (swingTimer > 0)
        {
            transform.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed, Space.World);
            swingTimer -= Time.deltaTime;
            yield return null;
        }
    }
}
