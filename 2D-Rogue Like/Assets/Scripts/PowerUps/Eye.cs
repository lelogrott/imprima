using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour, IInventoryItem {

	public string Name
	{
		get
		{
			return "Eye";
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
