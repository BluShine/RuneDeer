using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingPhysics : MonoBehaviour {

    Rigidbody body;
    public Vector3 upDir = Vector3.up;

    public float standingForce = 10;
    public float kP = .2f;
    public float kI = .05f;
    public float kD = .1f;
    public float iBound = 1;

    float lastError = 0;
    float integral = 0;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        upDir.Normalize();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        float error = Vector3.Angle(transform.TransformDirection(upDir), Vector3.up);
        float deriv = (error - lastError) / Time.fixedDeltaTime;
        integral += error * Time.fixedDeltaTime;
        integral = Mathf.Clamp(integral, -iBound, iBound);
        lastError = error;
        float force = kP * error + kI * integral + kD * deriv;
        force = Mathf.Clamp(force, -1, 1);

        // get its cross product, which is the axis of rotation to
        // get from one vector to the other
        Vector3 cross = Vector3.Cross(transform.TransformDirection(upDir), Vector3.up);
        // apply torque along that axis according to the magnitude of the angle.
        body.AddTorque(cross * force * standingForce);
    }

    private void OnDrawGizmosSelected()
    {
        upDir.Normalize();
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(upDir));
    }
}
