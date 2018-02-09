using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingPhysics : MonoBehaviour {

    Rigidbody body;
    public Vector3 upDir = Vector3.up;
    public float standingForce = 10;

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
        float angleDiff = Vector3.Angle(transform.TransformDirection(upDir), Vector3.up);

        // get its cross product, which is the axis of rotation to
        // get from one vector to the other
        Vector3 cross = Vector3.Cross(transform.TransformDirection(upDir), Vector3.up);

        // apply torque along that axis according to the magnitude of the angle.
        body.AddTorque(cross * angleDiff * standingForce);
    }
}
