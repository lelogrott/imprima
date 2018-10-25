using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBomb : MonoBehaviour {

    public int playerDamage;
    public int health;
    public float distanceToPlay = 2.5f;
	public AudioClip bombSound;
	private AudioSource audio;
    private Transform target;
    private bool defeated = false;
	private float timer = 0f;
    
    // Use this for initialization
    public void Start ()
	{
        target = GameObject.Find("Player").transform;
		audio = gameObject.GetComponent<AudioSource>();
        audio.mute = true;
		audio.clip = bombSound;
        
    }

	void Update ()
	{
        double distance = Math.Sqrt(Math.Pow(transform.position.x - target.position.x, 2) + Math.Pow(transform.position.y - target.position.y, 2));
		if (distance < distanceToPlay && target.gameObject.GetComponent<Player>().SoundPowerUp)
            audio.mute = false;
        timer += Time.deltaTime;
		//Debug.LogWarning("distancia: " + distance);
		double beepPitch = 1/(1 + distance);
        // Debug.LogWarning("pitch: " + beepPitch);
        // Debug.LogWarning(">> timer: " + timer);
        audio.volume = 0.8f - (float)distance / 3;
        //if (distance
        if (timer > 0.1)
		{
			audio.pitch = (float) ( 10 * beepPitch);
			timer = 0;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().GetDamage();
			gameObject.SetActive(false);
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

