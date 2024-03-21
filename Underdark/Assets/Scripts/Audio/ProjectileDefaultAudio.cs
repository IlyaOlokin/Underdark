using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProjectileDefaultAudio : MonoBehaviour
{
    [SerializeField] private List<AudioClip> createClips;
    [SerializeField] private List<AudioClip> deathClips;
    
    private AudioSource audioSource;
    private Projectile projectile;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        projectile = GetComponent<Projectile>();
    }

    private void OnEnable()
    {
        projectile.OnCreate += PlayCreateSound;
        projectile.OnDeath += PlayDeathSound;
    }

    private void PlayCreateSound()
    {
        if (createClips.Count == 0) return;
        audioSource.PlayOneShot(createClips[Random.Range(0, createClips.Count)]);
    }
    private void PlayDeathSound()
    {
        if (deathClips.Count == 0) return;
        audioSource.PlayOneShot(deathClips[Random.Range(0, deathClips.Count)]);
    }
    
    private void OnDisable()
    {
        projectile.OnCreate -= PlayCreateSound;
        projectile.OnDeath -= PlayDeathSound;

    }
}
