using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
			// Debug.LogWarning(">> slots available -> item: " + collider.enabled);			
			// if (collider.enabled)
			if (true)
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
}
