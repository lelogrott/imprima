using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void PlayGame()
	{
        int sc = SceneManager.GetActiveScene().buildIndex;
        if (sc == 0)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
        
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
