using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Push", fileName = "New PushInfo")]
public class PushInfo : DebuffInfo
{
    public float Force;
    public Vector2 DirectionOffset;
    private Vector2 pushDir;
    
    public override void Execute(IAttacker caster, Unit receiver)
    {
        pushDir =  receiver.transform.position - ((MonoBehaviour) caster).transform.position;
        
        receiver.GetPushed( this, pushDir.normalized * Force);
    }
}
