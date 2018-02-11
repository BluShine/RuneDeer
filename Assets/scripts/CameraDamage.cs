using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraDamage : MonoBehaviour {

    public float damageMultiplier = 10;
    public float damage = 0;
    public float damageCooldown = 5;
    float cooldownTimer = 0;
    public float knockbackForce = 100;

    public Texture2D dirtTexture;

    public ParticleSystem particles;

    PostProcessVolume volume;
    Bloom bloomEffect;
    public Rigidbody body;

    bool damaging = false;
    Vector3 damagePos = Vector3.zero;

    void Start()
    {
        bloomEffect = ScriptableObject.CreateInstance<Bloom>();
        //bloomEffect.dirtTexture.Override(dirtTexture);
        bloomEffect.enabled.Override(true);
        bloomEffect.dirtIntensity.Override(damage);

        volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, bloomEffect);
    }

    void Update()
    {
        cooldownTimer = Mathf.Max(0, cooldownTimer - Time.deltaTime);
        if(damaging)
        {
            damaging = false;
            if(cooldownTimer == 0)
            {
                damage++;
                cooldownTimer = damageCooldown;
                particles.Play();
                body.AddForce(((transform.position - damagePos).normalized + Vector3.up).normalized * knockbackForce, ForceMode.Impulse);
            }
        }
        bloomEffect.dirtIntensity.value = damage * damageMultiplier;
    }

    void Destroy()
    {
        RuntimeUtilities.DestroyVolume(volume, true);
    }

    public void HitBullet(Vector3 pos)
    {
        damaging = true;
        damagePos = pos;
    }
}
