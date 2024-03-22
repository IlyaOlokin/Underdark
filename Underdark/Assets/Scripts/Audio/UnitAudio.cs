using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class UnitAudio : MonoBehaviour
{
    [SerializeField] private List<AudioClip> damagedClips;
    [SerializeField] private List<AudioClip> deathClips;
    [SerializeField] private AudioClip levelUpClip;
    
    private AudioSource audioSource;
    private Unit unit;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        unit = GetComponent<Unit>();
    }

    private void OnEnable()
    {
        unit.OnDamaged += PlayDamagedSound;
        unit.OnDeath += PlayDeathSound;
        unit.Stats.OnLevelUp += PlayLevelUpSound;
    }

    private void PlayDamagedSound()
    {
        if (damagedClips.Count == 0) return;
        audioSource.PlayOneShot(damagedClips[Random.Range(0, damagedClips.Count)]);
    }
    
    private void PlayDeathSound()
    {
        if (deathClips.Count == 0) return;
        MyAudioSource.PlayClipAtPoint(deathClips[Random.Range(0, deathClips.Count)], transform.position,
            audioSource.outputAudioMixerGroup, audioSource.volume);
    }

    private void PlayLevelUpSound()
    {
        audioSource.PlayOneShot(levelUpClip);
    }
    
    private void OnDisable()
    {
        unit.OnDamaged -= PlayDamagedSound;
        unit.OnDeath -= PlayDeathSound;
        unit.Stats.OnLevelUp -= PlayLevelUpSound;
    }
}
