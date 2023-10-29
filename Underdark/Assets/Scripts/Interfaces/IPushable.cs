using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable
{
    bool IsPushing { get; }
    
    bool GetPushed(PushInfo pushInfo, Vector2 pushDir);

    void EndPush();
}