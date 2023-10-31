using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindowUI : MonoBehaviour
{
    private Player player;
    private UnitStats stats = new UnitStats();

    private event Action OnStatsChanged;

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
            player.GetExp(100);
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
    }

    private void ResetStatsUI()
    {
        stats.CopyStatsFrom(player.Stats);
        OnStatsChanged?.Invoke();
    }

    private void ApplyStats()
    {
        player.Stats.CopyStatsFrom(stats);
        OnStatsChanged?.Invoke();
    } 

    private void UpdateUI()
    {
        var haveFreePoints = stats.FreePoints > 0;
        strPlusButton.interactable = haveFreePoints;
        dexPlusButton.interactable = haveFreePoints;
        intPlusButton.interactable = haveFreePoints;
        
        strMinusButton.interactable = stats.Strength != player.Stats.Strength;
        dexMinusButton.interactable = stats.Dexterity != player.Stats.Dexterity;
        intMinusButton.interactable = stats.Intelligence != player.Stats.Intelligence;

        var statsChanged = stats != player.Stats;
        confirmChangesButton.interactable = statsChanged;
        canselChangesButton.interactable = statsChanged;

        strText.text = stats.Strength.ToString();
        dexText.text = stats.Dexterity.ToString();
        intText.text = stats.Intelligence.ToString();
        FreePointsText.text = stats.FreePoints.ToString();
        LevelText.text = player.Stats.Level.ToString();
    }

    private void AddStr()
    {
        if (stats.FreePoints <= 0) return;
        stats.Strength++;
        stats.FreePoints--;
        OnStatsChanged?.Invoke();
    }
    
    private void SubtractStr()
    {
        stats.Strength--;
        stats.FreePoints++;
        OnStatsChanged?.Invoke();
    }
    
    private void AddDex()
    {
        if (stats.FreePoints <= 0) return;
        stats.Dexterity++;
        stats.FreePoints--;
        OnStatsChanged?.Invoke();
    }
    
    private void SubtractDex()
    {
        stats.Dexterity--;
        stats.FreePoints++;
        OnStatsChanged?.Invoke();
    }
    
    private void AddInt()
    {
        if (stats.FreePoints <= 0) return;
        stats.Intelligence++;
        stats.FreePoints--;
        OnStatsChanged?.Invoke();
    }
    
    private void SubtractInt()
    {
        stats.Intelligence--;
        stats.FreePoints++;
        OnStatsChanged?.Invoke();
    }

    private void OnDisable()
    {
        OnStatsChanged -= UpdateUI;
    }
}
