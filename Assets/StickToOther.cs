using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToOther : MonoBehaviour {

    bool stuck;

	// Use this for initialization
	void Start () {
        stuck = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!stuck && collision.transform.tag.Equals("Sticky"))
        {
            transform.parent = collision.transform;
            GetComponent<Rigidbody2D>().isKinematic = true;
            Rigidbody2D[] bodies = GetComponentsInChildren<Rigidbody2D>();
            foreach(Rigidbody2D body in bodies)
            {
                body.isKinematic = false;
            }

            /*Transform bottom = collision.gameObject.GetComponent<Transform>();
            parent = bottom;*/

            stuck = true;
        }
    }
}
