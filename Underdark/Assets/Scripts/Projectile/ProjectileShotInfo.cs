using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileShotInfo
{
    [field:SerializeField] public float Shots { get; private set; }
    [field:SerializeField] public float ProjCountInShot { get; private set; }
    [field:SerializeField] public float AngleBetweenProj { get; private set; }
}
