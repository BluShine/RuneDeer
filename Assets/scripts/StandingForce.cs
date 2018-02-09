using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingForce : MonoBehaviour {

    Rigidbody body;

    public Vector3 direction = Vector3.down;
    public LayerMask mask;
    public Vector3 offset = Vector3.zero;

    public float maxDist = 10;
    public float targetDist = 5;

    public float maxForce = 10;
    public float kP = .2f;
    public float kI = .05f;
    public float kD = .1f;

    float lastError = 0;
    float integral = 0;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        if (body == null)
        {
            Debug.Log("missing rigidbody");
        }
        direction.Normalize();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position + transform.TransformDirection(offset), transform.TransformDirection(direction));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, maxDist, mask))
        {
            float error = hit.distance - targetDist;
            float deriv = (error - lastError) / Time.fixedDeltaTime;
            integral += error * Time.fixedDeltaTime;
            lastError = error;
            float force = kP * error + kI * integral + kD * deriv;
            force = Mathf.Clamp(force, -1, 0);
            body.AddForce(-force * maxForce * Vector3.up);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + transform.TransformDirection(offset) + transform.TransformDirection(direction) * targetDist, 
            transform.position + transform.TransformDirection(offset) + transform.TransformDirection(direction) * maxDist);
    }
}
