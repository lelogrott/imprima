using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour {

	public Inventory Inventory;
	public MessagePanel messagePanel;

	// Use this for initialization
	void Start () {
		Inventory.ItemAdded += InventoryScript_ItemAdded;
	}

	private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
	{
		Transform inventoryPanel = transform.Find("Inventory");
		foreach (Transform slot in inventoryPanel)
		{
			// border... image
			Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
			
			// we found the empty slot
			if (!image.enabled)
			{
				image.enabled = true;
				image.sprite = e.Item.Image;

				break;
			}
		}
	}

	public void OpenMessage()
	{
		messagePanel.gameObject.SetActive(true);
	}

	public void HideMessage()
	{
		messagePanel.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
