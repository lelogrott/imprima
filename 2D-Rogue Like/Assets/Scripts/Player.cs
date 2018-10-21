using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
// public class Player : MovingObject {

    public Inventory inventory;
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public float speed = 2;
    public float horizontal;
    public float vertical;
    public Text foodText;
    public Text timeText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    public GameObject Laser;
    public float laserForce;

    private Animator animator;
    private int food;
    private float totalTime = 20f;
    private bool goingBack = false;
    private Vector2 lastMovement;
    private Rigidbody2D rb2d;
    private Transform rangedSource;



    void Start () {
        rb2d = GetComponent<Rigidbody2D> ();
        rb2d.freezeRotation = true;
        animator = GetComponent<Animator>();
        // Debug.LogWarning("entered player start");
        food = GameManager.instance.playerFoodPoints;
        totalTime = GameManager.instance.totalTimeLeft;
        inventory.setMItems(GameManager.instance.inventoryItems);
        rangedSource = transform.Find("RangedAttackSource");

        foodText.text = "Food: " + food;

	}

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
        GameManager.instance.totalTimeLeft = totalTime;
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
        if (!GameManager.instance.getDoingSetup() && totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            if (totalTime < 0) totalTime = 0;
            var coloredTime = "<color=#ff0000ff>" + totalTime.ToString("0.00") + "</color>";
            
            // use red color for time if its lower then 10 seconds left
            if (totalTime < 10)
                timeText.text = "Tempo: " + coloredTime;
            else
                timeText.text = "Tempo: " + totalTime.ToString("0.00");

            CheckIfGameOver();
        }

        if(Input.GetKeyDown("space"))
        {
            animator.SetTrigger("playerRanged");
            CheckIfGameOver();
            fireLaser();
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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(other.tag == "Back")
        {
            goingBack = true;
            Invoke("Restart", restartLevelDelay);
            enabled = false;
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
            Debug.LogWarning(other);
        }

        IInventoryItem item = other.GetComponent<IInventoryItem> ();
        if (item != null)
        {
            inventory.AddItem(item);
        }
    }

    // protected override void OnCantMove<T>(T component)
    // {
    //     Wall hitWall = component as Wall;
    //     hitWall.DamageWall(wallDamage);
    //     animator.SetTrigger("playerChop");
    // }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
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
