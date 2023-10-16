using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int Strength { get; private set; }
    [field: SerializeField] public int Dexterity { get; private set; }
    [field: SerializeField] public int Intelligence { get; private set; }
}
