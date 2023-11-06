using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Zenject;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera;
    private Transform player;

    [Inject]
    private void Construct(Player player)
    {
        
        camera.Follow = player.transform;
    }

    private void Start()
    {
        //camera.Follow = player;
    }
}
