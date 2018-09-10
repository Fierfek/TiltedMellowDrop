using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToOther : MonoBehaviour {

    bool alive;
    bool stuck;
    float burnTime;

	// Use this for initialization
	void Start () {
        alive = true;
        stuck = false;
        burnTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

        if (transform.position.y < 0 && alive)
        {
            GetComponent<MallowAnimation>().burn();
            if (burnTime == 0.0f)
                burnTime = Time.time;
            else
                if(Time.time - burnTime >= 3.5)
            {
                Kill();
            }

            if (transform.position.y <= -6.0)
            {
                if (alive)
                {
                    //lose life
                }

                Destroy(gameObject);
            }
        }
    }

    public bool getAlive()
    {
        return alive;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<MallowAnimation>().impact();
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
