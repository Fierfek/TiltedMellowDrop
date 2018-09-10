using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StickToOther : MonoBehaviour {

    public class StuckEvent : UnityEvent<Transform> { }

    public StuckEvent stuckEvent = new StuckEvent();
    bool alive;
    bool stuck;

	// Use this for initialization
	void Start () {
        alive = true;
        stuck = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (transform.position.y <= -4.0)
        {
            if(alive)
            {
                //lose life
            }

            Destroy(gameObject);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!stuck && collision.transform.tag.Equals("Sticky"))
        {
            //transform.parent = collision.transform;
            GetComponent<Rigidbody2D>().isKinematic = true;
            Rigidbody2D[] bodies = GetComponentsInChildren<Rigidbody2D>();
            foreach(Rigidbody2D body in bodies)
            {
                body.isKinematic = false;
            }

            stuck = true;
            stuckEvent.Invoke(transform);
        }
    }

    void Kill()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        BoxCollider2D[] boxes = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D box in boxes)
        {
            box.enabled = false;
        }
    }
}
