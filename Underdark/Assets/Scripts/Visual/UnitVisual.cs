using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitVisual : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private SpriteRenderer sr;
    
    [Header("Alert")]
    [SerializeField] private GameObject alertMark;
    [SerializeField] private float alertDuration;
    

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
        alertMark.SetActive(false);
    }

    public void StartWhiteOut()
    {
        StartCoroutine(WhiteOut());
    }

    IEnumerator Alert()
    {
        alertMark.SetActive(true);
        yield return new WaitForSeconds(alertDuration);
        alertMark.SetActive(false);
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

    public void StartHighLightActiveAbility(ActiveAbility activeAbility, Unit owner)
    {
        float dist;
        float angle;
        
        dist = activeAbility.AttackDistance.GetValue(owner.GetExpOfActiveAbility(activeAbility.ID));
        angle = activeAbility.AttackAngle.GetValue(owner.GetExpOfActiveAbility(activeAbility.ID));
        
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
