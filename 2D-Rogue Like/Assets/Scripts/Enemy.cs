using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int playerDamage;
    public int enemyHealth;
    public float chaseRange = 5f;
    public float attackRange = .8f;
    public float speed = .5f;
    public float moveTime = 1f;
    public float facingAngle;
    public LayerMask blockingLayer;
    public BoxCollider2D boxCollider;
    private Animator animator;
    private Transform target;
    private bool skipMove;
    private bool defeated = false;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    public AudioClip chopSound1;
    public AudioClip chopSound2;



    // Use this for initialization
    void Start () {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        target = GameObject.FindGameObjectWithTag ("Player").transform;
	}

    void FixedUpdate()
    {
        MoveEnemy();
        Debug.LogWarning(facingAngle);
    }

    private void attack (RaycastHit2D obstacle)
    {
        Player hitPlayer = obstacle.transform.GetComponent<Player>();
        animator.SetTrigger("enemyAttack");
        hitPlayer.LoseFood(playerDamage);
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }

    private void chaseTarget()
    {
        Vector3 targetDir = target.position - transform.position;
        facingAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        flipEnemy(facingAngle);
        transform.Translate(targetDir * Time.deltaTime * speed);
    }

    private void flipEnemy(float angle)
    {
        if (angle > -70f && angle < 70f)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;
    }

    private bool targetOnSight(out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = target.transform.position;
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform.gameObject.tag == "Player") return true;
        return false;
    }

    public void MoveEnemy()
    {
        RaycastHit2D obstacle;
        if (targetOnSight(out obstacle))
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange)
                attack(obstacle);
            else if(distanceToTarget < chaseRange)
                chaseTarget();        
        }
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
