using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallProj : Projectile
{
    [SerializeField] private ActiveAbilityProperty<float> explosionRadius; 
    [SerializeField] private Collider2D explosionCollider; 
    
    [Header("Visual")] 
    [SerializeField] private GameObject lightSpot;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private List<ParticleSystem> deathExplosion;
    [SerializeField] private float destroyDelay = 1.5f;

    protected override void Die()
    {
        coll.enabled = false;
        rb.velocity = Vector2.zero;
        lightSpot.SetActive(false);

        sr.enabled = false;

        foreach (var system in deathExplosion)
        {
            var scale = explosionRadius.GetValue(abilityLevel);
            system.transform.localScale = new Vector3(scale, scale);
            system.Play();
        }

        if (abilityLevel > 1)
        {
            Explode();
        }
        
        CancelInvoke(nameof(Die));
        Destroy(gameObject, destroyDelay);
    }

    public override void Attack(IDamageable damageable)
    {
        if (abilityLevel <= 1)
            base.Attack(damageable);
    }

    private void Explode()
    {
        //explosionCollider.enabled = true;
        
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(caster.AttackMask);
        List<Collider2D> hits = new List<Collider2D>();

        Physics2D.OverlapCircle(transform.position, explosionRadius.GetValue(abilityLevel) + 0.5f, contactFilter, hits);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                base.Attack(damageable);
            }
        }
    }
}
