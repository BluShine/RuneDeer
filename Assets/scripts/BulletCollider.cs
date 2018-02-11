using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour {

    CameraDamage screen;
    ParticleSystem particles;
    List<ParticleSystem.Particle> inside;


    // Use this for initialization
    void Start () {
        screen = FindObjectOfType<CameraDamage>();
        particles = GetComponent<ParticleSystem>();
        inside = new List<ParticleSystem.Particle>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnParticleTrigger()
    {
        int numInside = particles.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
        if (numInside >= 1)
            screen.HitBullet(transform.position);
        //Debug.Log("trigger particle");
    }

}
