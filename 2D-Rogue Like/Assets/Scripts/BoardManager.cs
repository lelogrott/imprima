using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

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
	public List<GameObject> layoutObjects = new List<GameObject>();
	public List<Vector3> layoutPositions = new List<Vector3>();

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
		boardHolder = new GameObject ("Board").transform;

		for (int x = -1; x < columns + 1; x++) 
		{
			for (int y = -1; y < rows + 1; y++) 
			{
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerWallsTiles [Random.Range (0, outerWallsTiles.Length)];

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
					
			}
		}
		// Debug.LogWarning(boardHolder);
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
		BoardSetup ();
		InitialiseList ();
		if (GameManager.instance.boardDict.ContainsKey(level))
			loadMap (level);
		else
		{
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			int enemyCount = (int)Mathf.Log (level, 2f);
			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
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
