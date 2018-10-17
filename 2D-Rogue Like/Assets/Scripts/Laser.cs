using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

	private LineRenderer lineRenderer;
	public Transform LaserHit;

	// Use this for initialization
	void Start ()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = true;
		// lineRenderer.useWorldSpace = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
		Debug.DrawLine(transform.position, hit.point);
		LaserHit.position = hit.point;
		
		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(1, LaserHit.position);

		if ( hit.collider != null )
        {
			if (hit.collider.tag == "Player")
            {
				hit.collider.gameObject.GetComponent<Player>().LoseFood(10);
			}
        }
	}
}
