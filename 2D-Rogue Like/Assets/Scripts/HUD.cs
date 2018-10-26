using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour {

	public Inventory Inventory;
	public MessagePanel messagePanel;

	private List<string> messages = new List<string>();
	private List<string> titles = new List<string>();

	// Use this for initialization
	void Start () {
		titles.Add("IMPRIMA");
		messages.Add("<b>Missão</b>\n> Encontre e colete projetos privados de próteses aprimoradas da PROTEBRAS.\n> Suba ao 10º andar para acessar a antena e distribuir os projetos na internet.\n<b>Controles</b>\n> Movimento: UP, DOWN, LEFT, RIGHT ou W, A, S, D\n> Tiro Laser: ESPAÇO (mira com cursor)\n> Golpe Físico: Movimento + Q");
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

	public void OpenMessage(int messageIndex)
	{
		messagePanel.Title.text = titles[0];
		messagePanel.Content.text = messages[0];
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
