using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HUD : MonoBehaviour {

	public Inventory Inventory;
	public MessagePanel messagePanel;

	private List<string> messages = new List<string>();
	private List<string> titles = new List<string>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        //titles.Add("IMPRIMA");
        //messages.Add("<b>Missão</b>\n> Encontre e colete projetos privados de próteses aprimoradas da PROTEBRAS.\n> Suba ao 10º andar para acessar a antena e distribuir os projetos na internet.\n<b>Controles</b>\n> Movimento: UP, DOWN, LEFT, RIGHT ou W, A, S, D\n> Tiro Laser: ESPAÇO (mira com cursor)\n> Golpe Físico: Movimento + Q");

        AddTitlesToList(
            "IMPRIMA", //Aviso level 1
            "NOVA PROTESE: JOELHOS ELÁSTICOS", //Poder localizado no 2 andar
            "NOVA PROTESE: VISÂO AGUÇADA", //Poder localizado na sala secreta 2 andar
            "NOVA PROTESE: TÍMPANO ULTRASÔNICO", //Poder localizado no 7 andar
            "NOVA PROTESE: JUNTAS DE BRAÇO REFORÇADOS", //Poder localizado no 5 andar
            "CUIDADO!", //Aviso level 3
            "CUIDADO!", //Aviso level 7
            "CUIDADO!", //Aviso level 6
            "MISSÂO CUMPRIDA" //Computador na antena
            );
        AddMessagesToList(
            "<b> Missão </b>\n > Encontre e colete projetos privados de próteses aprimoradas da PROTEBRAS.\n > Suba ao 10º andar para acessar a antena e distribuir os projetos na internet.\n <b> Controles </b>\n > Movimento: UP, DOWN, LEFT, RIGHT ou W, A, S, D\n > Tiro Laser: ESPAÇO(mira com cursor)\n > Golpe Físico: Movimento + Q",
            "Permitem uma mobilidade extremamente rápida.", //Poder localizado no 2 andar
            "Permite enxergar uma maior gama de cores.", //Poder localizado na sala secreta 2 andar
            "Permite ouvir frequências ultrasônicas.", //Poder localizado no 7 andar
            "Proporciona a força de 10 homens, podendo até mesmo quebrar algumas paredes.", //Poder localizado no 5 andar
            "Campo de testes de laser, precisa ser desumanamente rápido.", //Aviso level 3
            "Pisos com minas terrestres ultrasônicas, emitem uma frequência de som abaixo da detecção humana.", //Aviso level 7            );
            "Pisos com laser infra vermelho (invisível ao olho nú)", //Aviso level 6
            "<b> A PROTEBRAS FOI HACKEADA </b>\n > Parabéns! \n Você conseguiu vazar todos os projetos das próteses para a população!" //Computador na antena
            );
        Inventory.ItemAdded += InventoryScript_ItemAdded;
	}

    void AddTitlesToList(params string[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            titles.Add(list[i]);
        }
    }

    void AddMessagesToList(params string[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            messages.Add(list[i]);
        }
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
        int levelOffset = 1;
        Debug.Log(messages.Count);
        messagePanel.Title.text = titles[messageIndex - levelOffset];
        messagePanel.Content.text = messages[messageIndex - levelOffset];
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
