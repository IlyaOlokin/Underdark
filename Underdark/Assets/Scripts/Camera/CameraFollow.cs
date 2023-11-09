using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtCamera;
    private Transform player;

    [Inject]
    private void Construct(Player player)
    {
        virtCamera.Follow = player.transform;
    }

    private void Start()
    {
        //camera.Follow = player;
    }
}
