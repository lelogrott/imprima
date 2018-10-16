using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public Inventory inventory;
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public Text timeText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    public GameObject Axe;
    public float axeForce;

    private Animator animator;
    private int food;
    private float totalTime = 20f;
    private bool goingBack = false;
    private Vector2 lastMovement;



    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();
        // Debug.LogWarning("entered player start");
        food = GameManager.instance.playerFoodPoints;
        totalTime = GameManager.instance.totalTimeLeft;
        inventory.setMItems(GameManager.instance.inventoryItems);

        foodText.text = "Food: " + food;

        base.Start();
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
            LoseFood(10);
            CheckIfGameOver();
            ThrowAxe();
        }
        if (!GameManager.instance.playersTurn) return;
        GameManager.instance.playersTurn = false;
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);

       

	}

    private void ThrowAxe()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        GameObject newAxe = Instantiate(Axe, transform.position + new Vector3(lastMovement.x, lastMovement.y), transform.rotation) as GameObject;

        newAxe.GetComponent<Rigidbody2D>().AddForce(lastMovement*axeForce);
        
        return;
    }
    
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // food--;
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move (xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        lastMovement = new Vector2(xDir, yDir);

        CheckIfGameOver();


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

        IInventoryItem item = other.GetComponent<IInventoryItem> ();
        if (item != null)
        {
            Debug.LogWarning(">> detected eye " + item);
            inventory.AddItem(item);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

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
