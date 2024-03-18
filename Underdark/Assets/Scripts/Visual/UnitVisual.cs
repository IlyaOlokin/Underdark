using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitVisual : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private SpriteRenderer sr;

    [Header("EraseEffect")] 
    [SerializeField] private float startEraseValue; 
    [SerializeField] private float endEraseValue; 
    [SerializeField] private GameObject damageEffect; 
    [SerializeField] private GameObject deathEffect; 
    [SerializeField] private GameObject circleDeathEffect; 
    private static readonly int EraseAmount = Shader.PropertyToID("_EraseAmount");
    
    [Header("Alert")]
    [SerializeField] private GameObject alertMark;
    [SerializeField] private float alertDuration;
    
    [Header("WhiteOut")]
    [SerializeField] private float whiteOutDuration;
    [SerializeField] private float whiteOutAmount;
    private static readonly int WhiteOutProp = Shader.PropertyToID("_WhiteOut");

    [Header("Debuffs")] 
    public GameObject StunBar;
    
    [Header("Death Imposter")]
    [SerializeField] private DeathImposter deathImposter;

    [Header("AbilityHighLight")] 
    [SerializeField] private GameObject highLightZone;
    [SerializeField] private Material highLightZoneMat;
    
    private static readonly int Turn = Shader.PropertyToID("_Turn");
    private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");

    private void Awake()
    {
        sr.material = new Material(mat);
        mat = sr.material;
        
        if (highLightZone != null) highLightZone.GetComponent<SpriteRenderer>().material = highLightZoneMat;
    }

    private void OnEnable()
    {
        mat.SetFloat(WhiteOutProp, 0);
        mat.SetFloat(EraseAmount, 0);
    }

    public void UpdateErase(IAttacker attacker, float healthProportion, bool needVisual = true)
    {
        var newValue = Mathf.Lerp(startEraseValue, endEraseValue, 1 - healthProportion);
        mat.SetFloat(EraseAmount, newValue);
        
        return;
        
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
            mat.SetFloat(WhiteOutProp, whiteOutAmount);
            yield return null;
        }
        mat.SetFloat(WhiteOutProp, 0);
    }

    public void StartHighLightActiveAbility(ActiveAbility activeAbility, Unit owner)
    {
        float dist;
        float angle;
        
        dist = activeAbility.AttackDistance.GetValue(owner.GetExpOfActiveAbility(activeAbility.ID));
        angle = activeAbility.AttackAngle.GetValue(owner.GetExpOfActiveAbility(activeAbility.ID));
        
        highLightZone.SetActive(true);
        highLightZone.transform.localScale = new Vector3(dist * 2 + 1, dist * 2 + 1);
        highLightZoneMat.SetFloat(Turn, 90);
        highLightZoneMat.SetFloat(FillAmount, angle);
    }

    public void EndHighLightActiveAbility()
    {
        highLightZone.SetActive(false);
    }

    public void StartDeathEffect(IAttacker attacker, DamageType damageType)
    {
        if (attacker != null)
        {
            var dir = transform.position - attacker.Transform.position;
            float exactAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + damageEffect.transform.eulerAngles.z;
            Instantiate(deathEffect, transform.position, Quaternion.Euler(0, 0, exactAngle));
        }
        else
        {
            Instantiate(circleDeathEffect, transform.position, Quaternion.identity);
        }
    }
}
