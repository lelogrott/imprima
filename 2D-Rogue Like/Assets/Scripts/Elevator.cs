using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

	private Transform player;
	public float distanceToActivate;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
        Debug.Log("HERE");
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerDistance() < distanceToActivate)
        {
            GetComponent<Collider2D>().enabled = true;
            Debug.Log("enabled");
        }
	}

	private float PlayerDistance()
	{
		return Vector3.Distance(transform.position, player.position);
	}
}
