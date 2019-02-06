using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction;

    private bool holding;
    private bool moving_right;
    private float left_bounds;
    private float right_bounds;
    private GameObject marshmallow_instance;
    private List<GameObject> marshmallows;
    bool usingAndroid = Application.platform == RuntimePlatform.Android;
    private Camera cam;

    public float speed;
    public GameObject marshmallow_prefab;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        moving_right = true;
        right_bounds = cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x;
        left_bounds = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        MobileHelper.ins.tapEvent.AddListener((x) => DropMarshmallow());
        holding = false;
        direction = Vector2.right;
        if (usingAndroid) speed /= 2;
        marshmallows = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        move();
        if (holding)
        {
            marshmallow_instance.transform.position = transform.position;
            if (Input.GetMouseButtonUp(0))
                DropMarshmallow();
        }
        else
        {
            if (marshmallows.Count >= 5)
            {
                bool searching = true;
                do
                {
                    if (marshmallows.Contains(null))
                        marshmallows.Remove(null);
                    else
                        searching = false;


                } while (searching);


                /*for (int i = 0; i < marshmallows.Count; i)
                {
                    if(marshmallows.)
                }*/
            }
            else
                stacked();
        }
    }

    void DropMarshmallow()
    {
        marshmallow_instance.GetComponent<MallowAnimation>().fall();
        holding = false;
        marshmallow_instance.GetComponent<Rigidbody2D>().WakeUp();
        marshmallow_instance.GetComponent<Rigidbody2D>().freezeRotation = false;

        marshmallow_instance.GetComponent<BoxCollider2D>().enabled = true;
        BoxCollider2D[] boxes = marshmallow_instance.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D box in boxes)
        {
            box.enabled = true;
        }

        marshmallow_instance.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
        Rigidbody2D[] bodies = marshmallow_instance.GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D body in bodies)
        {
            body.velocity = new Vector2(0.0f, 0.0f);
        }

        marshmallows.Add(marshmallow_instance);
        marshmallow_instance = null;
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
        marshmallow_instance.GetComponent<Rigidbody2D>().freezeRotation = true;

        marshmallow_instance.GetComponent<BoxCollider2D>().enabled = false;
        BoxCollider2D[] boxes = marshmallow_instance.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D box in boxes)
        {
            box.enabled = false;
        }

        holding = true;
    }
}
