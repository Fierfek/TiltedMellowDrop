using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesnHeight : MonoBehaviour {
    public Text LifeCount;
    public Text HeightCount;

    public BoxCollider2D DropZone;
    public BoxCollider2D PenaltyZone;
 
    // Use this for initialization
    int lives;
    int height;
	void Start () {
        height = 0;
        lives = 3;
        LifeCount.text = lives.ToString();
        HeightCount.text = height.ToString();

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
        LifeCount.text = lives.ToString();
        HeightCount.text = height.ToString();
        if(lives == 0)
        {

        }
    }
}
