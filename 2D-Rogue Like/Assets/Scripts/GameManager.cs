using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    public MessagePanel hud;
    public bool wasGameOver = false;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private int maxLevel = 10;
    private bool enemiesMoving;
    private bool doingSetup;
    
    private List<string> messages = new List<string>();
	private List<string> titles = new List<string>();
    
    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        hud = GetComponent<MessagePanel>();
    }

    void Start() {
        AddTitlesToList(
            "IMPRIMA", //Aviso level 1
            "NOVA PROTESE: JOELHOS ELÁSTICOS", //Poder localizado no 2 andar
            "NOVA PROTESE: VISÂO AGUÇADA", //Poder localizado na sala secreta 2 andar
            "NOVA PROTESE: TÍMPANO ULTRASÔNICO", //Poder localizado no 7 andar
            "NOVA PROTESE: JUNTAS DE BRAÇO REFORÇADOS", //Poder localizado no 5 andar
            "CUIDADO!", //Aviso level 3
            "CUIDADO!", //Aviso level 7
            "CUIDADO!", //Aviso level 6
            "MISSÂO CUMPRIDA", //Computador na antena
            "???" 
            );
        AddMessagesToList(
            "<b> Missão </b>\n > Encontre e colete projetos privados de próteses aprimoradas da PROTEBRAS.\n > Suba ao 10º andar para acessar a antena e distribuir os projetos na internet.\n <b> Controles </b>\n > Movimento: UP, DOWN, LEFT, RIGHT ou W, A, S, D\n > Tiro Laser: ESPAÇO(mira com cursor)\n > Golpe Físico: Movimento + Q",
            "Permitem uma mobilidade extremamente rápida.", //Poder localizado no 2 andar
            "Permite enxergar uma maior gama de cores.", //Poder localizado na sala secreta 2 andar
            "Permite ouvir frequências ultrasônicas.", //Poder localizado no 7 andar
            "Proporciona a força de 10 homens, podendo até mesmo quebrar algumas paredes.", //Poder localizado no 5 andar
            "Campo de testes de laser, precisa ser desumanamente rápido.", //Aviso level 3
            "Pisos com minas terrestres ultrasônicas, emitem uma frequência de som abaixo da detecção humana.", //Aviso level 7            );
            "Pisos com laser infra vermelho (invisível ao olho nú)", //Aviso level 6
            "<b> A PROTEBRAS FOI HACKEADA </b>\n > Parabéns! \n Você conseguiu vazar todos os projetos das próteses para a população!", //Computador na antena
            "Its Just Carrot juice"
            );
    }

    public void OpenMessage(int messageIndex)
    {
        TextMeshProUGUI Title = GameObject.Find("Canvas").transform.Find("MessagePanel").transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI> ();
		TextMeshProUGUI Content = GameObject.Find("Canvas").transform.Find("MessagePanel").transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI> ();
        int levelOffset = 1;
        Debug.Log(messages.Count);
        Title.text = titles[messageIndex - levelOffset];
        Content.text = messages[messageIndex - levelOffset];
		GameObject.Find("Canvas").transform.Find("MessagePanel").gameObject.GetComponent<CanvasGroup>().alpha = 1;
    }

    public void HideMessage()
	{
		GameObject.Find("Canvas").transform.Find("MessagePanel").gameObject.GetComponent<CanvasGroup>().alpha = 0;
	}

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        instance.InitGame();
        //if (!wasGameOver)
        //{
            instance.level++;
            //StartCoroutine(addLevel());
        //}
    }

    IEnumerator addLevel()
    {
        yield return new WaitForSeconds(2f);
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
        if (!wasGameOver)
        {
            
            levelText.text = level + "° andar";
        }
        else
        {
            levelText.text = "1° andar";
        }
        
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
        //enabled = false;
        wasGameOver = true;
        StartCoroutine(returnToLevelOne());
    }

    IEnumerator returnToLevelOne()
    {
        yield return new WaitForSeconds(1.5f);
        specialItemCounter = 0;
        inventoryItems = new List<IInventoryItem>();
        playerStartPosition = new Vector3(7, 0, 0f);
        level = 1;
        SceneManager.LoadScene(0);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator returnToMainMenu()
    { 
        yield return new WaitForSeconds(1.5f);
        Application.LoadLevel(1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

    public void AddTitlesToList(params string[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            titles.Add(list[i]);
        }
    }

    public void AddMessagesToList(params string[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            messages.Add(list[i]);
        }
    }
}
