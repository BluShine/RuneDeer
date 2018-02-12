using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkCam : MonoBehaviour {

    public float moveAccel = 50;
    public float maxSpeed = 50;
    public float jumpForce = 30;
    public float airAccel = 10;
    static string fAxis = "Vertical";
    static string hAxis = "Horizontal";
    static string vAxis = "Hover";

    Rigidbody body;

    public float groundHeight = 1;
    public LayerMask mask;

    float jumpTimer = 0;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionStay(Collision collision)
    {
        
    }

    void FixedUpdate()
    {
        float accel = airAccel;
        if(Physics.Raycast(transform.position, Vector3.down, groundHeight, mask))
        {
            accel = moveAccel;
            jumpTimer += Time.fixedDeltaTime;
            if(jumpTimer > .1f && Input.GetAxis(vAxis) > 0)
            {
                body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpTimer = 0;
            }
        } else
        {
            jumpTimer = 0;
        }
        Vector3 hVel = new Vector3(body.velocity.x, 0, body.velocity.z);
        if (hVel.magnitude > maxSpeed)
        {
            body.AddForce(-hVel.normalized * accel);
        } else
        {
            Vector3 inputVec = new Vector3(Input.GetAxis(hAxis), 0, Input.GetAxis(fAxis));
            if(inputVec.magnitude > 1)
            {
                inputVec.Normalize();
            }

            if(inputVec.magnitude == 0)
            {
                //stopping
                if(hVel.magnitude <= accel * Time.fixedDeltaTime)
                {
                    body.AddForce(-hVel, ForceMode.VelocityChange);
                } else
                {
                    body.AddForce(-hVel.normalized * accel);
                }
            } else
            {
                body.AddForce(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * inputVec * accel);
            }
        }
    }
}
