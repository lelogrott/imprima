using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour {

	public Inventory Inventory;

    // Use this for initialization

	void Awake ()
	{
		DontDestroyOnLoad(GameObject.Find("Canvas").transform.FindChild("MessagePanel"));
	}
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

	// Update is called once per frame
	void Update () {
		
	}
}
