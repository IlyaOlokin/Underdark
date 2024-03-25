using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class ObjectAudio : MonoBehaviour
{
    [SerializeField] private List<AudioClip> createClips;
    [SerializeField] private List<AudioClip> deathClips;
    [SerializedDictionary("Key", "Clip")] 
    public SerializedDictionary<string, AudioClip> customClips;
    
    private AudioSource audioSource;
    private ISoundEmitterOnCreate createSoundEmitter;
    private ISoundEmitterOnDeath deathSoundEmitter;
    private ISoundEmitterOnDeathSeparate deathSeparateSoundEmitter;
    private ISoundEmitterOnCustom customSoundEmitter;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        createSoundEmitter = GetComponent<ISoundEmitterOnCreate>();
        deathSoundEmitter = GetComponent<ISoundEmitterOnDeath>();
        deathSeparateSoundEmitter = GetComponent<ISoundEmitterOnDeathSeparate>();
        customSoundEmitter = GetComponent<ISoundEmitterOnCustom>();
    }

    private void OnEnable()
    {
        if (createClips.Count > 0 && createSoundEmitter != null) createSoundEmitter.OnCreateSound += PlayCreateSound;
        if (deathClips.Count > 0 && deathSoundEmitter != null) deathSoundEmitter.OnDeathSound += PlayDestroySound;
        if (deathClips.Count > 0 && deathSeparateSoundEmitter != null) deathSeparateSoundEmitter.OnDeathSeparateSound += PlayDeathSoundSeparate;
        if (customClips.Count > 0 && customSoundEmitter != null) customSoundEmitter.OnCustomSound += PlayCustomSound;
    }

    private void PlayCreateSound()
    {
        if (createClips.Count == 0) return;
        audioSource.PlayOneShot(createClips[Random.Range(0, createClips.Count)]);
    }
    private void PlayDestroySound()
    {
        if (deathClips.Count == 0) return;
        audioSource.PlayOneShot(deathClips[Random.Range(0, deathClips.Count)]);
    }
    
    private void PlayDeathSoundSeparate()
    {
        if (deathClips.Count == 0) return;
        MyAudioSource.PlayClipAtPoint(deathClips[Random.Range(0, deathClips.Count)], transform.position,
            audioSource.outputAudioMixerGroup, audioSource.volume);
    }
    
    private void PlayCustomSound(string key)
    {
        if (customClips.Count == 0) return;
        if (customClips.TryGetValue(key, out AudioClip clip))
            audioSource.PlayOneShot(clip);
    }
    
    private void OnDisable()
    {
        if (createClips.Count > 0 && createSoundEmitter != null) createSoundEmitter.OnCreateSound -= PlayCreateSound;
        if (deathClips.Count > 0 && deathSoundEmitter != null) deathSoundEmitter.OnDeathSound -= PlayDestroySound;
        if (deathClips.Count > 0 && deathSeparateSoundEmitter != null) deathSeparateSoundEmitter.OnDeathSeparateSound -= PlayDeathSoundSeparate;
        if (customClips.Count > 0 && customSoundEmitter != null) customSoundEmitter.OnCustomSound -= PlayCustomSound;
    }
}