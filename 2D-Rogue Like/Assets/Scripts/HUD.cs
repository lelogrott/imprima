using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour {

	public Inventory Inventory;

	// Use this for initialization
	void Start () {
		Inventory.ItemAdded += InventoryScript_ItemAdded;
	}

	private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
	{
		// Debug.LogWarning(">> HUD");
		Transform inventoryPanel = transform.Find("Inventory");
		foreach (Transform slot in inventoryPanel)
		{
			// border... image
			Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
			// Debug.LogWarning("image >> " + image);
			// Debug.LogWarning("image enabled >> " + image.enabled);
			// Debug.LogWarning("item image >> " + e.Item.Image);
			
			// we found the empty slot
			if (!image.enabled)
			{
				image.enabled = true;
				image.sprite = e.Item.Image;

				// TODO: STORE A REFERENCE TO THE ITEM
				break;
			}
		}
	}

	private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
	{
		Transform inventoryPanel = transform.Find("Inventory");
		foreach (Transform slot in inventoryPanel)
		{
			Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
			
			if (image.sprite == e.Item.Image)
			{
				image.sprite = null;
				image.enabled = false;
			}
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
