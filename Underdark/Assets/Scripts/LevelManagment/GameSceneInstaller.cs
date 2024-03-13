using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerEntryFromUpStartPos;
    [SerializeField] private Transform playerEntryFromDownStartPos;
    
    [Header("Inputs")]
    [SerializeField] private MobileInput mobileInputPrefab;
    [SerializeField] private DesktopInput desktopInputPrefab;

    [Header("UI")] 
    [SerializeField] private Canvas canvas;
    [SerializeField] private PlayerInputUI playerInputUIMobile;
    [SerializeField] private PlayerInputUI playerInputUIDesktop;
    [SerializeField] private PlayerUI playerUI;
    
    [SerializeField] private FastTravelUI fastTravelUI;
    [SerializeField] private LevelChooseUI levelChooseUI;
    [SerializeField] private VendorUI vendorUI;
    
    public override void InstallBindings()
    {
        BindPlayerUI();
        
        BindPlayerInputUI();
        BindInput();
        BindPlayer();

        BindFastTravelUI();
        BindLevelChooseUI();
        BindVendorUI();
    }

    private void BindVendorUI()
    {
        Container.Bind<VendorUI>().FromInstance(vendorUI).AsSingle();
    }

    private void BindFastTravelUI()
    {
        Container.Bind<FastTravelUI>().FromInstance(fastTravelUI).AsSingle();
    }
    
    private void BindLevelChooseUI()
    {
        Container.Bind<LevelChooseUI>().FromInstance(levelChooseUI).AsSingle();
    }
    
    private void BindPlayerUI()
    {
        Container.Bind<PlayerUI>().FromInstance(playerUI).AsSingle();
    }

    private void BindPlayerInputUI()
    {
        RectTransform rectTransform;
        switch (Bootstrap.InputType)
        {
            case InputType.Mobile:
                PlayerInputUI mobileUI = Container
                    .InstantiatePrefabForComponent<PlayerInputUI>(playerInputUIMobile, Vector3.zero, Quaternion.identity, canvas.transform);
                mobileUI.gameObject.SetActive(true);
                rectTransform = mobileUI.transform.GetComponent<RectTransform>();
                rectTransform.SetLeft(1030); // debug
                rectTransform.SetRight(1030); // debug
                rectTransform.SetTop(490); // debug
                rectTransform.SetBottom(490); // debug
                
                Container.Bind<PlayerInputUI>().FromInstance(mobileUI).AsSingle();
                break;
            case InputType.Desktop:
                PlayerInputUI desktopUI = Container
                    .InstantiatePrefabForComponent<PlayerInputUI>(playerInputUIDesktop, Vector3.zero, Quaternion.identity, canvas.transform);
                desktopUI.gameObject.SetActive(true);
                
                rectTransform = desktopUI.transform.GetComponent<RectTransform>();
                rectTransform.SetLeft(1030); // debug
                rectTransform.SetRight(1030); // debug
                rectTransform.SetTop(490); // debug
                rectTransform.SetBottom(490); // debug
                
                Container.Bind<PlayerInputUI>().FromInstance(desktopUI).AsSingle();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        var startPos = LevelTransition.StartFromUp
            ? playerEntryFromUpStartPos.position
            : playerEntryFromDownStartPos.position;
        
        Player player = Container
            .InstantiatePrefabForComponent<Player>(playerPrefab, startPos, Quaternion.identity, null);
        player.gameObject.SetActive(true);

        Container.Bind<Player>().FromInstance(player).AsSingle();
    }
}