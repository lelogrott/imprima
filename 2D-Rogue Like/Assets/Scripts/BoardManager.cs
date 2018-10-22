﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using Sharptile;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count(int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count (5, 9);
	public Count foodCount = new Count (1, 5);
	public GameObject exit;
	public GameObject back;
	public GameObject eye;
	public GameObject laserWeapon;
	public GameObject sonicBomb;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallsTiles;
    public GameObject[] tilesetTiles;
    public List<GameObject> layoutObjects = new List<GameObject>();
	public List<Vector3> layoutPositions = new List<Vector3>();
	private string gameDataFileName = "levels.json";
	private Map levels;
    
	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3> ();

	 void Update() 
	 {
		 DontDestroyOnLoad(gameObject);
	 }
	
	void InitialiseList()
	{
		gridPositions.Clear ();
		layoutObjects.Clear ();
		layoutPositions.Clear ();
		for (int x = 1; x < columns - 1; x++) 
		{
			for (int y = 1; y < rows - 1; y++) 
			{
				gridPositions.Add (new Vector3 (x, y, 0f));	
			}
		}
	}

	void BoardSetup()
	{
        //Debug.Log(levels.layers);
		boardHolder = new GameObject ("Board").transform;
        int loop = 0;
        for (int x = -1; x < 9; x++) 
		{
			for (int y = -1; y < rows + 1; y++) 
			{
                loop++;
                int newX = x + 1;
                int newY = y + 1;
                int index = (10 - newX - 1) * 10 + newY;
                
                if (loop < 101)
                {
                    //Debug.Log(index +" "+ newX + " " + newY);

                    if (levels.layers[0].data[index] != 0)
                    {
                        GameObject toInstantiate = tilesetTiles[levels.layers[0].data[index]];
                        GameObject instance = Instantiate(toInstantiate, new Vector3(y, x, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                    }

                    if (levels.layers[1].data[index] != 0)
                    {
                        GameObject tileChoice = tilesetTiles[levels.layers[1].data[index]];
                        Instantiate(tileChoice, new Vector3(y, x, 0f), Quaternion.identity);
                        layoutObjects.Add(tileChoice);
                        layoutPositions.Add(new Vector3(y, x, 0f));
                        Debug.Log("included" + levels.layers[1].data[index]);
                    }
                }
            }
		}
    }

    Vector3 RandomPosition()
	{
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = Random.Range (minimum, maximum + 1);

		for (int i = 0; i < objectCount; i++) 
		{
			Vector3 randomPosition = RandomPosition ();
            int index = Random.Range(0, tileArray.Length);

            GameObject tileChoice = tileArray [index];
			Instantiate (tileChoice, randomPosition, Quaternion.identity);
			layoutObjects.Add(tileChoice);
			layoutPositions.Add(randomPosition);
		}
		// Debug.LogWarning("LAYOUTS\nobjects:\n>> " + layoutObjects.Count + "\npositions:\n>> " + layoutPositions.Count);
		
	}


	public void SetupScene(int level)
	{
        LoadGameMaps();
        InitialiseList();
        BoardSetup();
		
        if (GameManager.instance.boardDict.ContainsKey(level))
        {
            loadMap(level);
        }
		else
		{
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			int enemyCount = (int)Mathf.Log (level, 2f);
			enemyCount = 3;
			//LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		}
		saveCurrentMap();
		if (level < GameManager.instance.getMaxLevel())
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0F), Quaternion.identity);
		if (level > 1)
			Instantiate (back, new Vector3 (0, rows - 1, 0F), Quaternion.identity);
		// Instantiate (eye, new Vector3 (columns - 1, 0, 0F), Quaternion.identity);
		// Instantiate (eye, new Vector3 (columns - 2, 0, 0F), Quaternion.identity);
		LayoutObjectAtRandom(new GameObject[] {eye, laserWeapon, sonicBomb}, 3, 3);
	}

	public void saveCurrentMap() {
		int level = GameManager.instance.getLevel();
		// Debug.LogWarning("SAVING >> " + this.layoutObjects.Count);
		// Debug.LogWarning(">> Saving floor number " + level);
		BoardManagerData boardData = new BoardManagerData(this, level);
		string boardDataAsJson = JsonUtility.ToJson(boardData);
		// Debug.LogWarning(">>>> Json string: " + boardDataAsJson);
		if (GameManager.instance.boardDict.ContainsKey(level))
			GameManager.instance.boardDict[level] = boardDataAsJson;
		else
			GameManager.instance.boardDict.Add(level, boardDataAsJson);
	}

	public void loadMap(int level) {
		string jsonString = GameManager.instance.boardDict[level];
		BoardManagerData boardData = JsonUtility.FromJson<BoardManagerData>(jsonString);
		// Debug.LogWarning(">> loading map: " + jsonString);
		this.layoutObjects = boardData.layoutObjects;
		this.layoutPositions = boardData.layoutPositions;
		for (int i = 0; i < layoutObjects.Count; i++)
		{
			Instantiate ((GameObject) layoutObjects[i], layoutPositions[i], Quaternion.identity);
		}
	}

    private void LoadGameMaps()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine("Assets/Maps/", gameDataFileName);

        if(File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath); 
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            //dynamic loadedData = JsonUtility.FromJson<dynamic>(dataAsJson);
            var loadedData = JsonUtility.FromJson<Map>(dataAsJson);
            // Retrieve the allRoundData property of loadedData
            levels = loadedData;

			//Debug.Log(levels.layers[0].name);
            //Debug.Log(levels.layers[1].name);
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }

    }
}

[Serializable]
public class BoardManagerData {
	public List<GameObject> layoutObjects = new List<GameObject>();
	public List<Vector3> layoutPositions = new List<Vector3>();
	public BoardManagerData(BoardManager bm, int level){
		this.layoutObjects = bm.layoutObjects;
		this.layoutPositions = bm.layoutPositions;
	}
}

public class LevelLayer
{
    public IList<int> data { get; set; }
    public int height { get; set; }
    public string name { get; set; }
    public int opacity { get; set; }
    public string type { get; set; }
    public bool visible { get; set; }
    public int width { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class LevelLayerGroup
{
    public IList<LevelLayer> layers { get; set; }
    public string name { get; set; }
    public int opacity { get; set; }
    public string type { get; set; }
    public bool visible { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class Tileset
{
    public int firstgid { get; set; }
    public string source { get; set; }
}

public class Levels
{
    public int height { get; set; }
    public bool infinite { get; set; }
    public IList<LevelLayerGroup> layers { get; set; }
    public int nextobjectid { get; set; }
    public string orientation { get; set; }
    public string renderorder { get; set; }
    public string tiledversion { get; set; }
    public int tileheight { get; set; }
    public IList<Tileset> tilesets { get; set; }
    public int tilewidth { get; set; }
    public string type { get; set; }
    public int version { get; set; }
    public int width { get; set; }
}
