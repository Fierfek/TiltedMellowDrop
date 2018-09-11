using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour {

	public AudioClip boomSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void playBoomSound() {

		AudioSource.PlayClipAtPoint(boomSound, transform.position, 0.75f);

	}
}
