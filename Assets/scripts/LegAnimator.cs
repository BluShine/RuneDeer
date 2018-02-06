using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegAnimator : MonoBehaviour {

    public Transform ikTarget;
    IK ikObject;
    public Transform hoof;
    public float rayDistance = 1;
    public float minDistance = .5f;
    public LayerMask raycastMask;
    public Vector3 neutralPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
        ikObject = ikTarget.GetComponent<IK>();
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, rayDistance, raycastMask) && hit.distance >= minDistance)
        {
            ikTarget.position = hit.point;
        }
        else
        {
            ikTarget.position = transform.TransformPoint(neutralPosition);
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.TransformPoint(neutralPosition), .1f);
        Gizmos.DrawLine(transform.position - transform.up * minDistance, transform.position - transform.up * rayDistance);
    }
}
