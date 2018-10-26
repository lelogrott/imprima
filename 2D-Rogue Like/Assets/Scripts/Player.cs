using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {

    public Inventory inventory;
    public int wallDamage = 1;
    public int meleePower = 1;
    public float restartLevelDelay = 1f;
    public float speed = 2;
    public float horizontal;
    public float vertical;
    public Text specialItemCounterText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public GameObject Laser;
    public float laserForce;

    // powerups control
    public bool StrengthPowerUp = false;
    public bool VisionPowerUp = false;
    public bool SpeedPowerUp = false;
    public bool SoundPowerUp = false;

    private bool invencible;
    private Animator animator;
    private int specialItemCounter;
    private bool goingBack = false;
    private Rigidbody2D rb2d;
    private Transform rangedSource;
    private bool stillAlive = true;
    private bool printing = false; 

    void Start () {
        rb2d = GetComponent<Rigidbody2D> ();
        rb2d.freezeRotation = true;
        animator = GetComponent<Animator>();
        // Debug.LogWarning("entered player start");
        specialItemCounter = GameManager.instance.specialItemCounter;
        inventory.setMItems(GameManager.instance.inventoryItems);
        rangedSource = transform.Find("RangedAttackSource");
        disableMelee();
        transform.position = GameManager.instance.playerStartPosition;
        specialItemCounterText.text = "x" + specialItemCounter;
        //Debug.Log(GameManager.instance.playerStartPosition);
    }

    private void OnDisable()
    {
        GameManager.instance.specialItemCounter = specialItemCounter;
        GameManager.instance.inventoryItems = new List<IInventoryItem> (inventory.getMItems());

        // check if player is going back to the previous room
        // if so, we need to decrease the level by 2, since we always increase it
        // by one at the beginning
        if (goingBack) 
            GameManager.instance.setLevel(GameManager.instance.getLevel() - 2);
        goingBack = false;
    }

    void FixedUpdate ()
    {
        if (!stillAlive)
            GameOver();
        horizontal = Input.GetAxisRaw("Horizontal") * speed;
        vertical = Input.GetAxisRaw("Vertical") * speed;
        if (horizontal < 0)
        {
            if (GetComponent<SpriteRenderer>().flipX == false)
            {
                Vector3 flipPosition = rangedSource.position;
                flipPosition.x += 0.54f;
                rangedSource.position = flipPosition;
            }
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (horizontal > 0)
        {
            if (GetComponent<SpriteRenderer>().flipX == true)
            {
                Vector3 flipPosition = rangedSource.position;
                flipPosition.x -= 0.54f;
                rangedSource.position = flipPosition;
            }
            GetComponent<SpriteRenderer>().flipX = false;
        }
        Vector2 movement = new Vector2 (horizontal, vertical);
        rb2d.velocity = movement * speed;
    }

    // Update is called once per frame
    void Update () {
        if (printing) return;
        if (SpeedPowerUp && speed == 2)
            speed = 3;
        else if (!SpeedPowerUp && speed == 3)
            speed = 2;
        if(Input.GetKeyDown("space"))
        {
            animator.SetTrigger("playerRanged");
            fireLaser();
        }
        if(Input.GetKeyDown("q"))
        {
            animator.SetTrigger("playerChop");
            meleeAttack();
        }
	}

    private void fireLaser()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        GameObject newShot = Instantiate(Laser, rangedSource.position, transform.rotation) as GameObject;
        newShot.transform.up = (rangedSource.position - mouse) * -laserForce;
        newShot.GetComponent<Rigidbody2D>().AddForce((rangedSource.position - mouse).normalized * -laserForce);
        return;
    }

    private void disableMelee()
    {
        transform.Find("MeleeAttackSourceRight").GetComponent<Collider2D>().enabled = false;
        transform.Find("MeleeAttackSourceLeft").GetComponent<Collider2D>().enabled = false;
        transform.Find("MeleeAttackSourceDown").GetComponent<Collider2D>().enabled = false;
        transform.Find("MeleeAttackSourceUp").GetComponent<Collider2D>().enabled = false;
    }

    private void meleeAttack()
    {
        if (horizontal > 0)
            transform.Find("MeleeAttackSourceRight").GetComponent<Collider2D>().enabled = true;
        if (vertical > 0)
            transform.Find("MeleeAttackSourceUp").GetComponent<Collider2D>().enabled = true;
        if (horizontal < 0)
            transform.Find("MeleeAttackSourceLeft").GetComponent<Collider2D>().enabled = true;
        if (vertical < 0)
            transform.Find("MeleeAttackSourceDown").GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit" || other.tag == "Back")
        {
            if (other.tag == "Back")
                goingBack = true;

            Invoke("Restart", restartLevelDelay);
            GameManager.instance.playerStartPosition = other.gameObject.transform.position;
        }
         else if (other.tag == "Wall")
        {
            if (StrengthPowerUp)
                other.SendMessage("DamageWall", wallDamage, SendMessageOptions.DontRequireReceiver);
        }
        else if (other.tag == "Enemy")
        {
            other.SendMessage("DamageEnemy", meleePower, SendMessageOptions.DontRequireReceiver);
        }
        else if (other.tag == "SpecialItem")
        {
            specialItemCounter++;
            specialItemCounterText.text = "x" + specialItemCounter;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "WarningMessage")
        {
            GameManager.instance.hud.OpenMessage(GameManager.instance.getLevel());
        }
        else if (other.tag == "Computer")
        {
            GameManager.instance.hud.OpenMessage(GameManager.instance.getLevel());
        }
        else if (other.tag == "Reprinter")
        {
            List<IInventoryItem> brokenItems = inventory.getBrokenItems();
            if (brokenItems.Count > 0 && specialItemCounter > 0)
            {
                StartCoroutine(startPrintAnimation());
                int randomIndex = Random.Range (0, brokenItems.Count);
                inventory.AddItem(brokenItems[randomIndex]);
                specialItemCounter--;
                specialItemCounterText.text = "x" + specialItemCounter;
            }
        }
        IInventoryItem item = other.GetComponent<IInventoryItem> ();
        if (item != null)
        {
            inventory.AddItem(item);
        }
        disableMelee();
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void GetDamage()
    {
        if (!invencible)
        {
            animator.SetTrigger("playerHit");
            stillAlive = inventory.removeLastAdded();
            StartCoroutine(setInvencible());
       }
        
    }

    public void ApplyPowerUp (string power)
    {
        switch(power)
        {
            case "Eye":
                VisionPowerUp = true;
                break;
            case "Sound":
                SoundPowerUp = true;
                break;
            case "Speed":
                SpeedPowerUp = true;
                break;
            case "Strength":
                StrengthPowerUp = true;
                break;
            default:
                break;
        }	
    }

    public void RemovePowerUp (string power)
    {
        switch(power)
        {
            case "Eye":
                VisionPowerUp = false;
                break;
            case "Sound":
                SoundPowerUp = false;
                break;
            case "Speed":
                SpeedPowerUp = false;
                break;
            case "Strength":
                StrengthPowerUp = false;
                break;
            default:
                break;
        }	
    }

    IEnumerator setInvencible()
    {
        invencible = true;
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 0f, 1.0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 0f, 1.0f, 1f);
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1f);
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1f);
        yield return new WaitForSeconds(0.2f);
        invencible = false;
    }

    IEnumerator startPrintAnimation()
    {
        printing = true;
        speed = 0;
        animator.SetTrigger("playerPrint");
        yield return new WaitForSeconds(1);
        printing = false;
        speed = 2;
    }

    private void GameOver()
    {
        GameManager.instance.GameOver();       
    }
}
