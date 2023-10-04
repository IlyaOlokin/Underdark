using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoBehaviour
{
    public static InputType InputType;
    
    public  void Awake()
    {
#if UNITY_ANDROID
        InputType = InputType.Mobile;
#endif
        
#if UNITY_EDITOR
        InputType = InputType.Mobile;
#endif
    }
}