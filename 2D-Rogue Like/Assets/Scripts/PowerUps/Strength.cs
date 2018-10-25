using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strength : MonoBehaviour, IInventoryItem {

	public string Name
	{
		get
		{
			return "Strength";
		}
	}

	public Sprite _Image = null;
	public Sprite Image
	{
		get
		{
			return _Image;
		}
	}

	public void OnPickup()
	{
		gameObject.SetActive(false);
	}
}
