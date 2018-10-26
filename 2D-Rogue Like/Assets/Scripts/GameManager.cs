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
    public int specialItemCounter = 0;
    [HideInInspector] public bool playersTurn = true;
    public List<IInventoryItem> inventoryItems = new List<IInventoryItem>();
    public Dictionary<int, string> boardDict = new Dictionary<int, string>();
    public Vector3 playerStartPosition = new Vector3(7, 0, 0f);
    public HUD hud;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private int maxLevel = 10;
    private bool enemiesMoving;
    private bool doingSetup;
    
    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        HUD hud = GameObject.Find("Canvas").GetComponent<HUD>();
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

        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "Você morreu!";
        levelImage.SetActive(true);
        enabled = false;
        StartCoroutine(returnToMainMenu());
    }

    IEnumerator returnToMainMenu()
    { 
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (doingSetup)
            return;
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
