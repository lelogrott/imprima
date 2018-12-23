using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShot : MonoBehaviour {


    public int damage;

    void Start()
    {
        Debug.LogWarning("positção x: " + this.transform.position.x + " posição y: " + this.transform.position.y);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food" || other.tag == "Soda" || other.tag == "Exit" || other.tag == "Back" || other.tag == "RedTile" || other.tag == "soundBomb") return;

        if (other.tag == "Enemy") other.SendMessage("DamageEnemy", damage, SendMessageOptions.DontRequireReceiver);
        Debug.LogWarning(other);
        if (other.tag != "Player") Destroy(gameObject);
        
    }

}
