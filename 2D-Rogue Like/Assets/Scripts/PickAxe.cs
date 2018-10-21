using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxe : MonoBehaviour {


    public int damage;

    void Update()
    {
        transform.Rotate(0, 0, -450 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food" || other.tag == "Soda") return;

        if (other.tag == "Wall") other.SendMessage("DamageWall", damage, SendMessageOptions.DontRequireReceiver);

        if (other.tag == "Enemy") other.SendMessage("DamageEnemy", damage, SendMessageOptions.DontRequireReceiver);

        if (other.tag != "Player") Destroy(gameObject);
        
    }

}
