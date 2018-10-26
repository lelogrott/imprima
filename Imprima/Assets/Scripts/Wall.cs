using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Sprite[] dmgSprite;
    private int hp;
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hp = dmgSprite.Length;
	}
	
	public void DamageWall(int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        hp -= loss;
        if (hp < 0)
            gameObject.SetActive(false);
        else
            spriteRenderer.sprite = dmgSprite[hp];
    }
}
