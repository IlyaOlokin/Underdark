using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitVisual : MonoBehaviour
{
    [SerializeField] private Material mat;
    private SpriteRenderer sr;
    
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
    
    [Header("Death Imposter")]
    [SerializeField] private DeathImposter deathImposter;

    [Header("AbilityHighLight")] 
    [SerializeField] private GameObject highLightZone;
    [SerializeField] private Material highLightZoneMat;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.material = new Material(mat);
        mat = sr.material;
        energyShieldSpriteRenderer = EnergyShield.GetComponent<SpriteRenderer>();
        if (highLightZone != null) highLightZone.GetComponent<SpriteRenderer>().material = highLightZoneMat;
    }

    private void OnEnable()
    {
        mat.SetFloat("_WhiteOut", 0);
        mat.SetFloat("_Thickness", 0);
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

    public void StartHighLightActiveAbility(ActiveAbility activeAbility, WeaponSO weapon)
    {
        float dist;
        float angle;
        
        if (activeAbility.NeedOverrideWithWeaponStats && weapon.ID != "empty")
        {
            dist = weapon.AttackDistance;
            angle = weapon.AttackRadius;
        }
        else
        {
            dist = activeAbility.AttackDistance;
            angle = activeAbility.AttackRadius;
        }
        
        highLightZone.SetActive(true);
        highLightZone.transform.localScale = new Vector3(dist * 2 + 1, dist * 2 + 1);
        highLightZoneMat.SetFloat("_Turn", 90);
        highLightZoneMat.SetFloat("_FillAmount", angle);
    }

    public void EndHighLightActiveAbility()
    {
        highLightZone.SetActive(false);
    }

    public void StartDeathEffect(IAttacker attacker, DamageType damageType)
    {
        var newImposter = Instantiate(deathImposter, transform.position, Quaternion.identity);
        newImposter.StartDeath(attacker, damageType, sr.sprite);
        newImposter.transform.eulerAngles = new Vector3(0, transform.parent.eulerAngles.y, 0);
    }
}
