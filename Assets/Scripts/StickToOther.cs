using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StickToOther : MonoBehaviour {

    public class StuckEvent : UnityEvent<Transform> { }

    public StuckEvent stuckEvent = new StuckEvent();
    public StuckEvent unstuckEvent = new StuckEvent();
    public StuckEvent killedEvent = new StuckEvent();
    bool alive = true;
    public bool stuck = false, landed = false;
    float burnTime = 0.0f;
    public bool willBurn = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if (transform.position.y < 0 && alive)
        {
            if (willBurn)
            {
                GetComponent<MallowAnimation>().burn();
                if (burnTime == 0.0f)
                    burnTime = Time.time;
                else
                    if (Time.time - burnTime >= 3.5)
                {
                    Kill();
                    alive = false;
                }
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
            stuckEvent.Invoke(transform);
            landed = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(stuck)
        {
            stuck = false;
            unstuckEvent.Invoke(transform);
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
        killedEvent.Invoke(transform);
    }
}
