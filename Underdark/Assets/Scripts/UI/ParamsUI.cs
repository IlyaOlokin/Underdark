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
        
        UpdateAmplifications();
        UpdateResistances();
    }

    private void UpdateAmplifications()
    {
        physDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Physic) - 1) * 100}%";
        chaosDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Chaos) - 1) * 100}%";
        fireDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Fire) - 1) * 100}%";
        airDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Air) - 1) * 100}%";
        waterDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Water) - 1) * 100}%";
        coldDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Cold) - 1) * 100}%";
        electricDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Electric) - 1) * 100}%";
    }
    
    private void UpdateResistances()
    {
        physDmgResText.text = $"{(1 - player.Params.GetDamageResistance(DamageType.Physic)) * 100}%";
        chaosDmgResText.text = $"{(1 - player.Params.GetDamageResistance(DamageType.Chaos)) * 100}%";
        fireDmgResText.text = $"{(1 - player.Params.GetDamageResistance(DamageType.Fire)) * 100}%";
        airDmgResText.text = $"{(1 - player.Params.GetDamageResistance(DamageType.Air)) * 100}%";
        waterDmgResText.text = $"{(1 - player.Params.GetDamageResistance(DamageType.Water)) * 100}%";
        coldDmgResText.text = $"{(1 - player.Params.GetDamageResistance(DamageType.Cold)) * 100}%";
        electricDmgResText.text = $"{(1 - player.Params.GetDamageResistance(DamageType.Electric)) * 100}%";
    }
}
