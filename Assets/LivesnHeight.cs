using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesnHeight : MonoBehaviour {
    public Text LifeCount;
    public Text HeightCount;
    public Text GameOver;
 
    // Use this for initialization
    int lives;
    int height;
	void Start () {
        height = 0;
        lives = 3;
        LifeCount.text = lives.ToString();
        HeightCount.text = height.ToString();
        GameOver.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            lives++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            lives--;
        }
        if (lives == 0)
        {
            GameOver.text = "Game Over";
        }
        LifeCount.text = lives.ToString();
    }
}
