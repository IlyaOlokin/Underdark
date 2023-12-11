using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ParamsUI : MonoBehaviour
{
    private Player player;
    
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI maxHpText;
    [SerializeField] private TextMeshProUGUI maxMpText;
    [SerializeField] private TextMeshProUGUI evasionText;

    [Header("Damage Amplifications")] 
    [SerializeField] private TextMeshProUGUI physDmgAmplText;
    [SerializeField] private TextMeshProUGUI chaosDmgAmplText;
    [SerializeField] private TextMeshProUGUI fireDmgAmplText;
    [SerializeField] private TextMeshProUGUI airDmgAmplText;
    [SerializeField] private TextMeshProUGUI waterDmgAmplText;
    [SerializeField] private TextMeshProUGUI coldDmgAmplText;
    [SerializeField] private TextMeshProUGUI electricDmgAmplText;
    
    [Header("Damage Resistances")] 
    [SerializeField] private TextMeshProUGUI physDmgResText;
    [SerializeField] private TextMeshProUGUI chaosDmgResText;
    [SerializeField] private TextMeshProUGUI fireDmgResText;
    [SerializeField] private TextMeshProUGUI airDmgResText;
    [SerializeField] private TextMeshProUGUI waterDmgResText;
    [SerializeField] private TextMeshProUGUI coldDmgResText;
    [SerializeField] private TextMeshProUGUI electricDmgResText;
    
    public void Init(Player player)
    {
        this.player = player;
    }
    
    private void OnEnable()
    {
        player.Inventory.OnEquipmentChanged += UpdateUI;
        player.Stats.OnStatsChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        player.Inventory.OnEquipmentChanged -= UpdateUI;
        player.Stats.OnStatsChanged -= UpdateUI;
    }
    
    private void UpdateUI()
    {
        maxHpText.text = $"{player.MaxHP}";
        maxMpText.text = $"{player.MaxMana}";
        evasionText.text = $"{Mathf.Round(player.Params.GetEvasionChance() * 100)}%";
        
        UpdateAmplifications();
        UpdateResistances();
    }

    private void UpdateAmplifications()
    {
        HandleAmplificationText(physDmgAmplText, DamageType.Physic);
        HandleAmplificationText(chaosDmgAmplText, DamageType.Chaos);
        HandleAmplificationText(fireDmgAmplText, DamageType.Fire);
        HandleAmplificationText(airDmgAmplText, DamageType.Air);
        HandleAmplificationText(waterDmgAmplText, DamageType.Water);
        HandleAmplificationText(coldDmgAmplText, DamageType.Cold);
        HandleAmplificationText(electricDmgAmplText, DamageType.Electric);
    }

    private void HandleAmplificationText(TextMeshProUGUI text, DamageType damageType)
    {
        text.text = $"{Mathf.RoundToInt((player.Params.GetDamageAmplification(damageType) - 1) * 100)}%";
    }
    
    private void UpdateResistances()
    {
        HandleResistanceText(physDmgResText, DamageType.Physic);
        HandleResistanceText(chaosDmgResText, DamageType.Chaos);
        HandleResistanceText(fireDmgResText, DamageType.Fire);
        HandleResistanceText(airDmgResText, DamageType.Air);
        HandleResistanceText(waterDmgResText, DamageType.Water);
        HandleResistanceText(coldDmgResText, DamageType.Cold);
        HandleResistanceText(electricDmgResText, DamageType.Electric);
    }

    private void HandleResistanceText(TextMeshProUGUI text, DamageType damageType)
    {
        text.text = $"{Mathf.RoundToInt((1 - player.Params.GetDamageResistance(damageType)) * 100)}%";

    }
}
