using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "DebuffInfos/Push", fileName = "New PushInfo")]
public class PushInfo : DebuffInfo
{
    public float Force;
    public PushType PushType;
    public float PushDuration;

    private Vector2 pushDir;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        if (Random.Range(0f, 1f) > chance) return;
        
        switch (PushType)
        {
            case PushType.Position:
                pushDir = receiver.transform.position -  attacker.Transform.position;
                break;
            case PushType.Rotation:
                var eulerAnglesZ = attacker.Transform.eulerAngles.z * Mathf.Deg2Rad;
                
                pushDir = new Vector2(Mathf.Cos(eulerAnglesZ), Mathf.Sin(eulerAnglesZ));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (receiver.TryGetComponent(out Push pushComponent))
        {
            receiver.EndPushState();
            Destroy(pushComponent);
        }
        
        var newPush = receiver.gameObject.AddComponent<Push>();
        newPush.Init(PushDuration, receiver, effectIcon);
        receiver.ReceiveStatusEffect(newPush);
        
        receiver.GetPushed(pushDir.normalized * Force);
    }

    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance pushes the target away.";
    }
}

public enum PushType
{
    Position,
    Rotation
}
