using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Push", fileName = "New PushInfo")]
public class PushInfo : DebuffInfo
{
    public float Force;
    public Vector2 DirectionOffset;
    public PushType PushType;
    private Vector2 pushDir;
    
    public override void Execute(IAttacker caster, Unit receiver, Unit unitCaster)
    {
        switch (PushType)
        {
            case PushType.Position:
                pushDir = receiver.transform.position -  caster.Transform.position;
                break;
            case PushType.Rotation:
                var eulerAnglesZ = (caster.Transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
                
                pushDir = new Vector2(Mathf.Cos(eulerAnglesZ), Mathf.Sin(eulerAnglesZ));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        receiver.GetPushed( this, pushDir.normalized * Force);
    }
}

public enum PushType
{
    Position,
    Rotation
}
