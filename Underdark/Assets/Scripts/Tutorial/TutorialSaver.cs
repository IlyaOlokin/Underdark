using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSaver : MonoBehaviour
{
    [SerializeField] private LevelTransition levelTransition;

    private void OnEnable()
    {
        levelTransition.OnLoad += CompleteTutorial;
    }

    private void CompleteTutorial()
    {
        LevelTransition.TutorialCompleted = true;
    }
    
    private void OnDisable()
    {
        levelTransition.OnLoad -= CompleteTutorial;
    }
}
