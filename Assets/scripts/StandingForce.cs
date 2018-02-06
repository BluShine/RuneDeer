using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingForce : MonoBehaviour {

    Rigidbody body;

    public Vector3 direction = Vector3.down;
    public float maxForce = 10;
    public float maxDist=2;
    public float minDist=1;
    public LayerMask mask;
    public Vector3 offset = Vector3.zero;

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
            Debug.Log("hit");
            float compression = Mathf.Min(1, 1 - (hit.distance - minDist) / (maxDist - minDist));
            body.AddForce(compression * maxForce * -transform.TransformVector(direction));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + transform.TransformDirection(offset) + transform.TransformDirection(direction) * minDist, transform.position + transform.TransformDirection(offset) + transform.TransformDirection(direction) * maxDist);
    }
}
