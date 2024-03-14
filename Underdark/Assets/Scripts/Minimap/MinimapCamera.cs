using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private LevelConfigSO levelConfig;
    void Start()
    {
        var currentFloorIndex = LevelTransition.GetCurrentFloorIndex();
        if (currentFloorIndex < 0) return;
        var levelSize = levelConfig.Floors[currentFloorIndex].LevelSize;
        minimapCamera.orthographicSize = levelSize / 2f;
    }
}
