using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSourceExplosion;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Shake(Vector3 pos)
    {
        cinemachineImpulseSource.GenerateImpulseAtPositionWithVelocity(pos, new Vector3(Random.Range(-0.15f, 0.15f),Random.Range(-0.15f, 0.15f), 0));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Shake(Vector3.zero);
    }

    public void ShakeExplosion(Vector3 pos)
    {
        cinemachineImpulseSourceExplosion.GenerateImpulseAtPositionWithVelocity(pos, new Vector3(Random.Range(-0.15f, 0.15f),Random.Range(-0.15f, 0.15f), 0));

    }
}
