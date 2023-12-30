using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSaver : MonoBehaviour
{
    [SerializeField] private LevelTransition levelTransition;

    private void Awake()
    {
        levelTransition.OnLoad += CompleteTutorial;
    }

    private void CompleteTutorial()
    {
        LevelTransition.TutorialCompleted = true;
    }
}
