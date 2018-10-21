using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.0f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    public float totalTimeLeft = 20f;
    [HideInInspector] public bool playersTurn = true;
    public List<IInventoryItem> inventoryItems = new List<IInventoryItem> ();
	public Dictionary<int, string> boardDict = new Dictionary<int, string>();


    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private int maxLevel = 10;
    private List<Enemy> enemies;
    private List<SonicBomb> bombs;
    private bool enemiesMoving;
    private bool doingSetup;

    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        bombs = new List<SonicBomb>();
        boardScript = GetComponent<BoardManager>();
        // InitGame();
    }


    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        instance.InitGame();
        instance.level++;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = level + "° andar";
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
       
        enemies.Clear();
        bombs.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver(string reason)
    {
        
        switch (reason)
        {
            case "Food":
                levelText.text = "After " + level + " days, you starved.";
                levelImage.SetActive(true);
                enabled = false;
                break;
            case "Time":
                levelText.text = "Acabou o tempo!\nVocê foi pego!";
                levelImage.SetActive(true);
                enabled = false;
                break;
        }

        
    }
	
	// Update is called once per frame
	void Update () {
        if (enemiesMoving || doingSetup)
            return;
        playersTurn = true;
        StartCoroutine(MoveEnemies());
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
    public void AddBombToList(SonicBomb script)
    {
        bombs.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(0.05f);
        // yield return new WaitForSeconds(0.0f);
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(0.05f);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    public int getLevel()
    {
        return level;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }

    public int getMaxLevel()
    {
        return maxLevel;
    }

    public bool getDoingSetup() {
        return doingSetup;
    }
}
