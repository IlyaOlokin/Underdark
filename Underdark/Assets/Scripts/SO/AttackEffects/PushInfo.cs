using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Push", fileName = "New PushInfo")]
public class PushInfo : DebuffInfo
{
    public float Force;
    public Vector2 DirectionOffset;
    public PushType PushType;
    private Vector2 pushDir;
    
    public override void Execute(IAttacker caster, Unit receiver)
    {
        switch (PushType)
        {
            case PushType.Position:
                pushDir = receiver.transform.position - ((MonoBehaviour) caster).transform.position;
                break;
            case PushType.Rotation:
                var eulerAnglesZ = (((MonoBehaviour)caster).transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
                
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
