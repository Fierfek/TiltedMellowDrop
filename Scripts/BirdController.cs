using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour {
    
    Vector2 targetPos, initPos;
    float f = 0, duration = 4; // speed at 4 = approx. 4 seconds to pass

	// Use this for initialization
	void Start () {
        initPos = transform.position;
        targetPos = initPos - new Vector2(2 * Camera.main.orthographicSize * Screen.width / Screen.height + 2, 0);
	}

    // Update is called once per frame
    void Update () {
        if (f < 1)
        {
            f += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initPos, targetPos, f);
        }
        else
            Destroy(gameObject);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
