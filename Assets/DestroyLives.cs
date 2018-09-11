using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLives : MonoBehaviour {

	public CheckMarshmallowDeath lifeHolder;
	int currentLives;

	// Use this for initialization
	void Start () {
		currentLives = lifeHolder.lives;
	}
	
	// Update is called once per frame
	void Update () {
		if(currentLives != lifeHolder.lives && lifeHolder.lives > 0) {
			transform.GetChild(lifeHolder.lives).GetComponent<SpriteRenderer>().enabled = false;
		}
	}
}
