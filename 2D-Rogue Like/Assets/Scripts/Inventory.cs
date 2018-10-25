using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour {

	private const int SLOTS = 4;
	private List<IInventoryItem> mItems = new List<IInventoryItem> ();

	public event EventHandler<InventoryEventArgs> ItemAdded;

	public void AddItem(IInventoryItem item)
	{
        if (hasItem(item.Name)) return;
	
        if (mItems.Count < SLOTS)
		{
			Collider2D collider = (item as MonoBehaviour).GetComponent<Collider2D>();
			if (collider.enabled)
			{
				Player player = GameObject.Find("Player").GetComponent<Player>();
				player.ApplyPowerUp(item.Name);
				collider.enabled = false;
				mItems.Add(item);		
				item.OnPickup();

				if (ItemAdded != null)
				{
					ItemAdded(this, new InventoryEventArgs(item));
				}
			}
		}
	}

	public void setMItems (List<IInventoryItem> itemsList)
	{
		foreach (IInventoryItem item in itemsList)
		{
			mItems.Add(item);
			Player player = GameObject.Find("Player").GetComponent<Player>();
			player.ApplyPowerUp(item.Name);
			if (ItemAdded != null)
			{
				ItemAdded(this, new InventoryEventArgs(item));
			}
		}
	}

	public List<IInventoryItem> getMItems ()
	{
		return mItems;
	}

	public bool hasItem (string name)
	{
		for (int i = 0; i < mItems.Count; i++)
			if (mItems[i].Name == name)
				return true;
		return false;
	}

	public bool removeLastAdded()
	{
		string itemRemoved = "";
		int totalItems = mItems.Count;
		if (totalItems != 0)
		{
			int lastItemIndex = mItems.Count - 1;
			Transform inventoryPanel = GameObject.Find("Inventory").transform;
			
			foreach (Transform slot in inventoryPanel)
			{
				Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
				if (image.sprite == mItems[lastItemIndex].Image)
				{
					image.sprite = null;
					image.enabled = false;
					break;
				}
			}
			itemRemoved = mItems[lastItemIndex].Name;
			mItems.RemoveAt(lastItemIndex);

			Player player = GameObject.Find("Player").GetComponent<Player>();
			player.RemovePowerUp(itemRemoved);

			return true;
		}
		return false;
	}
}
