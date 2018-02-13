using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerAI : MonoBehaviour {

    public StandingForce standing;
    public Rigidbody body;

    DeerState state = DeerState.Stand;
    float stateTimer = 5;

    static float STANDMIN = 20;
    static float STANDMAX = 40;
    static float CROUCHMIN = 3;
    static float CROUCHMAX = 5;

    float startHeight;
    static float LOWHEIGHT = 5;

    public float jumpForce = 1000;
    public float jumpHorizonalForce = 1000;
    public Vector3 jTorque = Vector3.one; //random limits of jumping torque

    public ParticleSystem[] particles;
    int activeParticleSys = 0;

    enum DeerState
    {
        Stand,
        Crouch,
        Jump
    };

	// Use this for initialization
	void Start () {
        startHeight = standing.targetDist;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        stateTimer -= Time.fixedDeltaTime;
		switch(state)
        {
            case DeerState.Stand:
                standing.targetDist = startHeight;
                if (stateTimer <= 0)
                {
                    stateTimer = Random.Range(CROUCHMIN, CROUCHMAX);
                    state = DeerState.Crouch;
                    particles[activeParticleSys].Stop();
                }
                break;
            case DeerState.Crouch:
                standing.targetDist = LOWHEIGHT;
                if(stateTimer <= 0)
                {
                    stateTimer = 3;
                    state = DeerState.Jump;
                    Jump();
                }
                break;
            case DeerState.Jump:
                if(stateTimer <= 0 && standing.isStanding())
                {
                    state = DeerState.Stand;
                    stateTimer = Random.Range(STANDMIN, STANDMAX);
                }
                break;
        }
	}

    void Jump()
    {
        Vector3 horizontal = Quaternion.Euler(0,Random.Range(-30, 30),0) * 
            new Vector3(-body.transform.position.x, 0, -body.transform.position.z).normalized;
        body.AddForce(horizontal * jumpHorizonalForce + Vector3.up * jumpForce, ForceMode.Impulse);
        body.AddTorque(new Vector3(
            Random.Range(-jTorque.x, jTorque.x), 
            Random.Range(-jTorque.y, jTorque.y), 
            Random.Range(-jTorque.z, jTorque.z)), 
            ForceMode.Impulse);
    }
}
