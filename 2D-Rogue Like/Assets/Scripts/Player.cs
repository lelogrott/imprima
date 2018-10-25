using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public Inventory inventory;
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int meleePower = 1;
    public float restartLevelDelay = 1f;
    public float speed = 2;
    public float horizontal;
    public float vertical;
    public Text foodText;
    public Text specialItemCounterText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;
    public GameObject Laser;
    public float laserForce;

    // powerups control
    public bool StrengthPowerUp = false;
    public bool VisionPowerUp = false;
    public bool SpeedPowerUp = false;
    public bool SoundPowerUp = false;

    public bool HaveStrengthPowerUp = false;
    public bool HaveVisionPowerUp = false;
    public bool HaveSpeedPowerUp = false;
    public bool HaveSoundPowerUp = false;

    private bool invencible;
    private Animator animator;
    private int specialItemCounter;
    private int food;
    private bool goingBack = false;
    private Rigidbody2D rb2d;
    private Transform rangedSource;

    void Start () {
        rb2d = GetComponent<Rigidbody2D> ();
        rb2d.freezeRotation = true;
        animator = GetComponent<Animator>();
        // Debug.LogWarning("entered player start");
        food = GameManager.instance.playerFoodPoints;
        specialItemCounter = GameManager.instance.specialItemCounter;
        Debug.LogWarning(inventory.getMItems().Count);
        inventory.setMItems(GameManager.instance.inventoryItems);
        rangedSource = transform.Find("RangedAttackSource");
        disableMelee();
        foodText.text = "Food: " + food;
        transform.position = GameManager.instance.playerStartPosition;
        specialItemCounterText.text = "x" + specialItemCounter;
        //Debug.Log(GameManager.instance.playerStartPosition);
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
        GameManager.instance.specialItemCounter = specialItemCounter;
        // Debug.LogWarning("setting gm inventory. b4: " + GameManager.instance.inventoryItems.Count);
        GameManager.instance.inventoryItems = new List<IInventoryItem> (inventory.getMItems());
        // Debug.LogWarning("setting gm inventory. now: " + GameManager.instance.inventoryItems.Count);
        // check if player is going back to the previous room
        // if so, we need to decrease the level by 2, since we always increase it
        // by one at the beginning
        if (goingBack) 
            GameManager.instance.setLevel(GameManager.instance.getLevel() - 2);
        goingBack = false;
        //playerStartPosition = rb2d.transform.position;
        //Debug.Log(playerStartPosition);
    }

    void FixedUpdate ()
    {
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
        if (SpeedPowerUp && speed == 2)
            speed = 3;
        if(Input.GetKeyDown("space"))
        {
            animator.SetTrigger("playerRanged");
            CheckIfGameOver();
            fireLaser();
        }
        if(Input.GetKeyDown("q"))
        {
            animator.SetTrigger("playerChop");
            CheckIfGameOver();
            meleeAttack();
            // disableMelee();
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
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
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
        else if (other.tag == "Reprinter")
        {
            //CRIA o ARRAY DE ALGUMA FORMA AQUI

            /*foreach (IInventoryItem currentItem in items)
            {
                if (!inventory.hasItem(currentItem.Name) && specialItemCounter > 0 && HaveVisionPowerUp)
                {
                    specialItemCounter--;
                    inventory.AddItem(currentItem);
                }
            }*/
            
            
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

    public void LoseFood(int loss)
    {
        if (!invencible)
        {
            animator.SetTrigger("playerHit");
            food -= loss;
            foodText.text = "-" + loss + " Food: " + food;
            /*bool morreu = inventory.removeLastAdded();
            if (morreu)
                foodText.text = ">>>> morreu <<<<";*/
            CheckIfGameOver();
            StartCoroutine(setInvencible());
       }
        
    }

    public void ApplyPowerUp (string power)
    {   Debug.LogWarning(power);
        switch(power)
        {
            case "Eye":
                VisionPowerUp = true;
                HaveVisionPowerUp = true;
                break;
            case "Sound":
                SoundPowerUp = true;
                HaveSoundPowerUp = true;
                break;
            case "Speed":
                SpeedPowerUp = true;
                HaveSpeedPowerUp = true;
                break;
            case "Strength":
                Debug.LogWarning("settingPowerUp");
                StrengthPowerUp = true;
                HaveStrengthPowerUp = true;
                break;
            default:
                break;
        }	
    }

    IEnumerator setInvencible()
    {
        Debug.Log(invencible);
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

    private void CheckIfGameOver()
    {
        // if (food <= 0) // if the food is over
        // {
        //     SoundManager.instance.PlaySingle(gameOverSound);
        //     SoundManager.instance.musicSource.Stop();
        //     GameManager.instance.GameOver("Food");
        // }
        // else if (!(totalTime > 0)) // if there is no time left
        // {
        //     SoundManager.instance.PlaySingle(gameOverSound);
        //     SoundManager.instance.musicSource.Stop();
        //     GameManager.instance.GameOver("Time");
        // }
            
    }
}
