using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    Sprite Icon { get; }
    float Duration { get; }
    float Timer { get; }
}
