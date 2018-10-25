using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

	private LineRenderer[] lineRenderers;
	private float counterToActivate = 0f;
	private float counterToDisable = 0f;
	private bool lasersDisabled = false;
	public Transform LaserHit;
	public float timeActive;
	public float timeDisabled;

	// Use this for initialization
	void Start ()
	{
		lineRenderers = GetComponents<LineRenderer>();
		for (int i = 0; i < lineRenderers.Length; i++)
			lineRenderers[i].enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		counterToDisable += Time.deltaTime;
		if (counterToActivate > timeDisabled)
		{
			counterToDisable = 0f;
			counterToActivate = 0f;
			lasersDisabled = false;
		}
		if (counterToDisable < timeActive)
			for (int i = 0; i < lineRenderers.Length; i++)
				renderLaser(lineRenderers[i]);
		else
		{
			if (!lasersDisabled)
			{
				for (int i = 0; i < lineRenderers.Length; i++)
					disableLaser(lineRenderers[i]);
				lasersDisabled = true;
			}
			counterToActivate += Time.deltaTime;
		}
	}

	private void renderLaser(LineRenderer lineRenderer)
	{
		RaycastHit2D hit;
		switch (lineRenderer.name)
		{
			case "SpawnUp":
				hit = Physics2D.Raycast(transform.position, transform.up);
				break;
			case "SpawnDown":
				hit = Physics2D.Raycast(transform.position, -transform.up);
				break;
			case "SpawnLeft":
				hit = Physics2D.Raycast(transform.position, -transform.right);
				break;
			case "SpawnRight":
				hit = Physics2D.Raycast(transform.position, transform.right);
				break;
			default:
				hit = Physics2D.Raycast(transform.position, transform.up);
				break;
		}
		Debug.DrawLine(transform.position, hit.point);
		LaserHit.position = hit.point;
		lineRenderer.enabled = true;
		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(1, LaserHit.position);

		if ( hit.collider != null )
        {
			if (hit.collider.tag == "Player")
            {
				hit.collider.gameObject.GetComponent<Player>().GetDamage();
			}
        }
	}
	private void disableLaser(LineRenderer lineRenderer)
	{
		lineRenderer.enabled = false;
	}
}
