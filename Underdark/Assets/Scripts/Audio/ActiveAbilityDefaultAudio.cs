using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class ActiveAbilityDefaultAudio : MonoBehaviour
{
    [SerializeField] private List<AudioClip> executeClips;
    
    private AudioSource audioSource;
    private ActiveAbility activeAbility;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        activeAbility = GetComponent<ActiveAbility>();
    }

    private void OnEnable()
    {
        activeAbility.OnExecute += PlaySound;
    }

    private void PlaySound()
    {
        audioSource.PlayOneShot(executeClips[Random.Range(0, executeClips.Count)]);
    }
    
    private void OnDisable()
    {
        activeAbility.OnExecute -= PlaySound;
    }
}
