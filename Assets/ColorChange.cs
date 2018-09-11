using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour {

	public Color[] colors;

	// Use this for initialization
	void Start () {
		Color col = colors[Random.Range(0, colors.Length)];
		GetComponent<SpriteRenderer>().color = col;
	}
}