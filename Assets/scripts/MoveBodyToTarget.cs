using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBodyToTarget : MonoBehaviour {

    public Rigidbody body;
    public float maxForce = 100;
    public Vector3 targetPos;
    public Transform target;

    public float kP = .2f;
    public float kI = .05f;
    public float kD = .1f;

    public float iBound = 1;

    Vector3 lastError = Vector3.zero;
    Vector3 integral = Vector3.zero;

    // Use this for initialization
    void Start () {
		
	}

    private void FixedUpdate()
    {
        Vector3 errorVector = target.position - body.position;
        errorVector = transform.InverseTransformVector(errorVector); //translate to local space for better behavior along each axis
        Vector3 deriv = (errorVector - lastError) / Time.fixedDeltaTime;
        integral += errorVector * Time.fixedDeltaTime;
        integral = new Vector3(Mathf.Clamp(integral.x, -iBound, iBound), 
            Mathf.Clamp(integral.y, -iBound, iBound), 
            Mathf.Clamp(integral.z, -iBound, iBound));
        lastError = errorVector;

        Vector3 force = kP * errorVector + kI * integral + kD * deriv;
        if(force.magnitude > 1)
        {
            force.Normalize();
        }
        body.AddForce(transform.TransformVector(force) * maxForce);
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(target.position, .1f);
    }
}
