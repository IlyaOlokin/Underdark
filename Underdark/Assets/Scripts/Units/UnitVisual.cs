using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitVisual : MonoBehaviour
{
    [SerializeField] private Material mat;
    [Header("Alert")]
    [SerializeField] private float maxThickness;
    [SerializeField] private float alertDuration;
    private IEnumerator alerting;
    private float thicknessMultiplier;

    [Header("WhiteOut")]
    [SerializeField] private float whiteOutDuration;
    [SerializeField] private float whiteOutAmount;

    [Header("Debuffs")] 
    public GameObject StunBar;

    [Header("Energy Shield")] 
    [SerializeField] private GameObject EnergyShield;
    private SpriteRenderer energyShieldSpriteRenderer;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().material = new Material(mat);
        mat = GetComponent<SpriteRenderer>().material;
        energyShieldSpriteRenderer = EnergyShield.GetComponent<SpriteRenderer>();
    }

    public void StartAlert()
    {
        StartCoroutine(Alert());
    }

    public void AbortAlert()
    {
        thicknessMultiplier = 0;
    }

    public void StartWhiteOut()
    {
        StartCoroutine(WhiteOut());
    }

    IEnumerator Alert()
    {
        float currentThickness = 0;
        thicknessMultiplier = 1;
        
        float speed = maxThickness / alertDuration;
        var alertTimer = alertDuration;

        while (alertTimer > 0)
        {
            currentThickness += speed * Time.deltaTime;
            alertTimer -= Time.deltaTime;
            mat.SetFloat("_Thickness", currentThickness * thicknessMultiplier);
            yield return null;
        }
        mat.SetFloat("_Thickness", 0);
    }
    
    IEnumerator WhiteOut()
    {
        var whiteOutTimer = whiteOutDuration;

        while (whiteOutTimer > 0)
        {
            whiteOutTimer -= Time.deltaTime;
            mat.SetFloat("_WhiteOut", whiteOutAmount);
            yield return null;
        }
        mat.SetFloat("_WhiteOut", 0);
    }

    public void ActivateEnergyShieldVisual(float radius)
    {
        EnergyShield.SetActive(true);
        energyShieldSpriteRenderer.material = new Material(energyShieldSpriteRenderer.material);
        
        energyShieldSpriteRenderer.material.SetFloat("_Turn", 90);
        energyShieldSpriteRenderer.material.SetFloat("_FillAmount", radius);
    }

    public void DeactivateEnergyShieldVisual()
    {
        EnergyShield.SetActive(false);
    }
}