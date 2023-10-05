using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerStartPos;
    
    [Header("Inputs")]
    [SerializeField] private MobileInput mobileInputPrefab;
    [SerializeField] private DesktopInput desktopInputPrefab;
    
    public override void InstallBindings()
    {
        BindInput();
        BindPlayer();
        //BindCamera();
    }

    private void BindInput()
    {
        switch (Bootstrap.InputType)
        {
            case InputType.Mobile:
                MobileInput mobileInput = Container
                    .InstantiatePrefabForComponent<MobileInput>(mobileInputPrefab, Vector3.zero, Quaternion.identity, null);
                mobileInput.gameObject.SetActive(true);
                
                Container.Bind<IInput>().FromInstance(mobileInput).AsSingle();
                break;
            case InputType.Desktop:
                DesktopInput desktopInput = Container
                    .InstantiatePrefabForComponent<DesktopInput>(desktopInputPrefab, Vector3.zero, Quaternion.identity, null);
                desktopInput.gameObject.SetActive(true);
                
                Container.Bind<IInput>().FromInstance(desktopInput).AsSingle();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void BindPlayer()
    {
        Player player = Container
            .InstantiatePrefabForComponent<Player>(playerPrefab, playerStartPos.position, Quaternion.identity, null);
        player.gameObject.SetActive(true);

        Container.Bind<Player>().FromInstance(player).AsSingle();
    }
}