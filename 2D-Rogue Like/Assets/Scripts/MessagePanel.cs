using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessagePanel : MonoBehaviour {

	public TextMeshProUGUI Title;
	public TextMeshProUGUI Content;
	public MessagePanel mp;

	private List<string> messages = new List<string>();
	private List<string> titles = new List<string>();

    // Use this for initialization
	// void Awake() {
    //     if (mp == null)
    //         mp = this;
    //     else if (mp != this)
    //         Destroy(gameObject);

    //     DontDestroyOnLoad(gameObject);
    // }

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

	public void OpenMessage(int messageIndex)
    {
        int levelOffset = 1;
        Debug.Log(messages.Count);
        Title.text = titles[messageIndex - levelOffset];
        Content.text = messages[messageIndex - levelOffset];
		Debug.LogWarning("dentro do mp>>" + this);
		this.mp.SetActive(true);
    }

    public void HideMessage()
	{
		// this.mp.SetActive(false);
	}

}
