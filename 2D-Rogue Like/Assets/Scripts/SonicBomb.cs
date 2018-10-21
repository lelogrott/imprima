using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBomb : MonoBehaviour {

    public int playerDamage;
    public int health;
	public AudioClip bombSound;
	private AudioSource audio;
    private Transform target;
    private bool defeated = false;
	private float timer = 0f;

    // Use this for initialization
    public void Start ()
	{
        GameManager.instance.AddBombToList(this);
        target = GameObject.FindGameObjectWithTag ("Player").transform;
		audio = gameObject.GetComponent<AudioSource>();
		audio.clip = bombSound;
	}

	void Update ()
	{
		timer += Time.deltaTime;
		double distance = Math.Sqrt(Math.Pow(transform.position.x - target.position.x, 2) + Math.Pow(transform.position.y - target.position.y, 2));
		// Debug.LogWarning("distancia: " + distance);
		double beepPitch = 1/(1 + distance);
		// Debug.LogWarning("pitch: " + beepPitch);
		// Debug.LogWarning(">> timer: " + timer);
		if (timer > 0.1)
		{
			audio.pitch = (float) ( 20 * beepPitch);
			timer = 0;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().LoseFood(playerDamage);
			// gameObject.SetActive(false);
        }
	}
	public void DamageEnemy(int loss)
    {
        health -= loss;
        if (health <= 0)
        {
            defeated = true;
            gameObject.SetActive(false);
        }
    }
}

