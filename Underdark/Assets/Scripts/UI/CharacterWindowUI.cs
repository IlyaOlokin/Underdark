using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindowUI : MonoBehaviour
{
    private Player player;
    public UnitStats Stats { get; private set; } = new UnitStats();

    private event Action OnStatsChanged;
    public event Action OnStatsApplied;

    [Header("StatsButton")]
    [SerializeField] private Button strPlusButton;
    [SerializeField] private Button strMinusButton;
    
    [SerializeField] private Button dexPlusButton;
    [SerializeField] private Button dexMinusButton;
    
    [SerializeField] private Button intPlusButton;
    [SerializeField] private Button intMinusButton;
    
    [Header("Buttons")]
    [SerializeField] private Button confirmChangesButton;
    [SerializeField] private Button canselChangesButton;
    [SerializeField] private Button LevelUpButton; // debug
    [SerializeField] private Button ResetButton;  // debug
    
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI strText;
    [SerializeField] private TextMeshProUGUI dexText;
    [SerializeField] private TextMeshProUGUI intText;
    [SerializeField] private TextMeshProUGUI FreePointsText;
    [SerializeField] private TextMeshProUGUI LevelText;
    
    [Space]
    [NonSerialized] public GameObject blackOut;

    
    public void Init(Player player)
    {
        this.player = player;
    }

    private void Awake()
    {
        ResetStatsUI();

        strPlusButton.onClick.AddListener(AddStr);
        strMinusButton.onClick.AddListener(SubtractStr);

        dexPlusButton.onClick.AddListener(AddDex);
        dexMinusButton.onClick.AddListener(SubtractDex);

        intPlusButton.onClick.AddListener(AddInt);
        intMinusButton.onClick.AddListener(SubtractInt);

        confirmChangesButton.onClick.AddListener(ApplyStats);
        canselChangesButton.onClick.AddListener(ResetStatsUI);
        
        // debug
        LevelUpButton.onClick.AddListener(() =>
        {
            player.Stats.GetExp(100);
            ResetStatsUI();
        });
        ResetButton.onClick.AddListener(() =>
        {
            player.Stats.Reset();
            ResetStatsUI(); 
        });
    }

    private void OnEnable()
    {
        OnStatsChanged += UpdateUI;
        ResetStatsUI();
        transform.SetAsLastSibling();
        blackOut.SetActive(true);
    }

    private void ResetStatsUI()
    {
        Stats.CopyStatsFrom(player.Stats);
        OnStatsChanged?.Invoke();
    }

    private void ApplyStats()
    {
        player.Stats.CopyStatsFrom(Stats);
        DataLoader.SaveGame(player);
        OnStatsChanged?.Invoke();
        OnStatsApplied?.Invoke();
    } 

    private void UpdateUI()
    {
        var haveFreePoints = Stats.FreePoints > 0;
        strPlusButton.interactable = haveFreePoints;
        dexPlusButton.interactable = haveFreePoints;
        intPlusButton.interactable = haveFreePoints;
        
        strMinusButton.interactable = Stats.Strength != player.Stats.Strength;
        dexMinusButton.interactable = Stats.Dexterity != player.Stats.Dexterity;
        intMinusButton.interactable = Stats.Intelligence != player.Stats.Intelligence;

        var statsChanged = Stats != player.Stats;
        confirmChangesButton.interactable = statsChanged;
        canselChangesButton.interactable = statsChanged;
        
        strText.text = player.Stats.BonusStrength == 0
            ? Stats.Strength.ToString()
            : $"{Stats.Strength} (+{player.Stats.BonusStrength})";
        
        dexText.text =  player.Stats.BonusDexterity == 0
            ? Stats.Dexterity.ToString()
            : $"{Stats.Dexterity} (+{player.Stats.BonusDexterity})";
        
        intText.text =  player.Stats.BonusIntelligence == 0
            ? Stats.Intelligence.ToString()
            : $"{Stats.Intelligence} (+{player.Stats.BonusIntelligence})";
        
        FreePointsText.text = $"{(Stats.FreePoints > 0 ? "<color=#FFD21A>": "")}{Stats.FreePoints}";
        LevelText.text = player.Stats.Level.ToString();
    }

    private void AddStr()
    {
        if (Stats.FreePoints <= 0) return;
        Stats.Strength++;
        Stats.FreePoints--;
        OnStatsChanged?.Invoke();
    }
    
    private void SubtractStr()
    {
        Stats.Strength--;
        Stats.FreePoints++;
        OnStatsChanged?.Invoke();
    }
    
    private void AddDex()
    {
        if (Stats.FreePoints <= 0) return;
        Stats.Dexterity++;
        Stats.FreePoints--;
        OnStatsChanged?.Invoke();
    }
    
    private void SubtractDex()
    {
        Stats.Dexterity--;
        Stats.FreePoints++;
        OnStatsChanged?.Invoke();
    }
    
    private void AddInt()
    {
        if (Stats.FreePoints <= 0) return;
        Stats.Intelligence++;
        Stats.FreePoints--;
        OnStatsChanged?.Invoke();
    }
    
    private void SubtractInt()
    {
        Stats.Intelligence--;
        Stats.FreePoints++;
        OnStatsChanged?.Invoke();
    }

    private void OnDisable()
    {
        OnStatsChanged -= UpdateUI;
        blackOut.SetActive(false);
    }
}
