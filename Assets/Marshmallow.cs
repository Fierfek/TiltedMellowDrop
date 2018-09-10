using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour {

    /*private bool alive;
    public int life;*/

	// Use this for initialization
	void Start () {
        /*alive = true;
        life = 5;*/
	}
	
	// Update is called once per frame
	void Update () {
        /*if (life <= 0 && alive)
           Kill();*/
	}

    /*public void Age()
    {
        life--;
    }*/

    /*void Kill ()
    {
        //mark this object as non-active
        alive = false;

        //TODO: CHANGE SPRITE TO BURNT

        //disable collision for all layers
        this.GetComponent<BoxCollider2D>().enabled = false;
        BoxCollider2D[] others = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D box in others)
            box.enabled = false;
    }*/
}
