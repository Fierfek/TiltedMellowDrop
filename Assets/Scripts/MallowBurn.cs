using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallowBurn : MonoBehaviour {
    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        Invoke("burn", 2f);
	}

    void burn()
    {
        anim.Play("burn");
    }
}
