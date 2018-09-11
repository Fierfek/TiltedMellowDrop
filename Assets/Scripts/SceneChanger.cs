using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    private void Start()
    {
        Screen.SetResolution(411, 731, false);
    }


    public void LoadGame() {
		SceneManager.LoadScene("MainGame");
	}

	public void MainMenu() {
		SceneManager.LoadScene("StartMenu");
	}

	public void Credits() {
		SceneManager.LoadScene("Credits");
	}

	public void TowerGame() {
		SceneManager.LoadScene("TowerBuild");
	}

	public void QuitGame() {
		Application.Quit();
	}

}