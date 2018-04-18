using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;
    public int enemyHealth;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    private bool defeated = false;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    public AudioClip chopSound1;
    public AudioClip chopSound2;



    // Use this for initialization
    protected override void Start () {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag ("Player").transform;
        base.Start();
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if(skipMove || defeated)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");

        hitPlayer.LoseFood(playerDamage);

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }

    public void DamageEnemy(int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        enemyHealth -= loss;
        if (enemyHealth <= 0)
        {
            defeated = true;
            gameObject.SetActive(false);
        }
    }
}
