using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ParamsUI : MonoBehaviour
{
    private Player player;

    [Header("Damage Amplifications")] 
    [SerializeField] private TextMeshProUGUI physDmgAmplText;
    [SerializeField] private TextMeshProUGUI chaosDmgAmplText;
    [SerializeField] private TextMeshProUGUI fireDmgAmplText;
    [SerializeField] private TextMeshProUGUI airDmgAmplText;
    [SerializeField] private TextMeshProUGUI waterDmgAmplText;
    [SerializeField] private TextMeshProUGUI coldDmgAmplText;
    [SerializeField] private TextMeshProUGUI electricDmgAmplText;
    
    public void Init(Player player)
    {
        this.player = player;
    }
    
    private void OnEnable()
    {
        player.Inventory.OnEquipmentChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        player.Inventory.OnEquipmentChanged -= UpdateUI;

    }
    
    private void UpdateUI()
    {
        physDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Physic) - 1) * 100}%";
        chaosDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Chaos) - 1) * 100}%";
        fireDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Fire) - 1) * 100}%";
        airDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Air) - 1) * 100}%";
        waterDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Water) - 1) * 100}%";
        coldDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Cold) - 1) * 100}%";
        electricDmgAmplText.text = $"{(player.Params.GetDamageAmplification(DamageType.Electric) - 1) * 100}%";
        
    }
}
