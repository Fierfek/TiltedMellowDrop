using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour {
    Rigidbody2D rb;
    Vector2 direction;

    private bool holding;
    private bool moving_right;
    private float left_bounds;
    private float right_bounds;
    private GameObject marshmallow_instance;
    private Camera cam;
    
    public float speed;
    public GameObject marshmallow_prefab;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        marshmallow_instance = Instantiate(marshmallow_prefab, transform.position, Quaternion.identity);
        moving_right = true;
        right_bounds = cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x;
        left_bounds = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        holding = true;
        direction = Vector2.right;
    }
	
	// Update is called once per frame
	void Update () {
        move();
        if (holding)
        {
            marshmallow_instance.transform.position = transform.position;
            if (Input.GetMouseButtonDown(0))
            {
                holding = false;
                marshmallow_instance.GetComponent<Rigidbody2D>().WakeUp();
                //GET RID OF LATER
                stacked();
            }
        }
	}

    void move()
    {
        rb.MovePosition(rb.position + (direction * speed * Time.deltaTime));
        Mathf.Clamp(rb.position.x, left_bounds, right_bounds);

        if (transform.position.x >= right_bounds)
        {
            direction = Vector2.left;
        }
        else if (transform.position.x <= left_bounds)
        {
            direction = Vector2.right;
        }

    }

    void stacked()
    {
        marshmallow_instance = Instantiate(marshmallow_prefab, transform.position, Quaternion.identity);
        holding = true;
    }
}
