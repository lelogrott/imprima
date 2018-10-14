using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour {

	private const int SLOTS = 7;
	private List<IInventoryItem> mItems = new List<IInventoryItem> ();

	public event EventHandler<InventoryEventArgs> ItemAdded;

	public void AddItem(IInventoryItem item)
	{
		// Debug.LogWarning(">> inventory add item");
		if (mItems.Count < SLOTS)
		{
			Collider2D collider = (item as MonoBehaviour).GetComponent<Collider2D>();
			// Debug.LogWarning(">> slots available -> item: " + collider.enabled);			
			// if (collider.enabled)
			if (true)
			{
				// Debug.LogWarning(">> adding item - total items before: " + mItems.Count);			
				collider.enabled = false;
				mItems.Add(item);
				// Debug.LogWarning(">> total items after: " + mItems.Count);			
				item.OnPickup();

				if (ItemAdded != null)
				{
					ItemAdded(this, new InventoryEventArgs(item));
				}
			}
		}
	}
}
