using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllableCrane : MonoBehaviour
{
    Vector2 direction;

    private bool holding;
    private GameObject marshmallow_instance;
    private List<GameObject> marshmallows;
    bool usingAndroid = Application.platform == RuntimePlatform.Android;
    private Camera cam;

    public float speed;
    public GameObject marshmallow_prefab;
    private bool movingCamera = false;
    private int marshmallowCount = 20;
    public bool forceStopCameraTrack = false;
    private bool canDrop = true;
    float currHeight = 0, currHeightRaw = 0;
    private GameObject  currentlyTrackedMallow;

    public Text heightText, remainingText;
	public GameObject heightCanvas, dropZone;

	// Use this for initialization
	void Start()
    {
        cam = Camera.main;
        SpawnHoldMallow();
        holding = true;
        direction = Vector2.right;
        if (usingAndroid) speed /= 2;
        marshmallows = new List<GameObject>();
        MobileHelper.ins.tapEvent.AddListener((x) => DropMarshmallow(x));
        MobileHelper.ins.holdReleaseEvent.AddListener((x) => DropMarshmallow(x));
        MobileHelper.ins.holdingEvent.AddListener((x) => ControlMarshmallow(x));
        MobileHelper.ins.panEvent.AddListener(() =>
        {
            forceStopCameraTrack = true;
            cam.transform.position = new Vector3(0, Mathf.Clamp(Mathf.Clamp(cam.transform.position.y, currHeightRaw + 1, 999), 0, 999), -10);
        });
    }

    // Update is called once per frame
    void Update()
    {
        currHeight = 0;
        foreach(GameObject x in marshmallows)
        {
            if (x.GetComponent<StickToOther>().landed)
            {
                var y = Math.Round((x.transform.position.y + 4) * 10, 1);
                if (y > currHeight)
                {
                    currHeightRaw = x.transform.position.y;
                    currHeight = (float)y;
                    currentlyTrackedMallow = x;
                }
            }
            /* DEBUG THE CURRENTLY TRACKED MALLOW
            if (currentlyTrackedMallow)
            {
                x.transform.Find("Debug").GetComponent<SpriteRenderer>().enabled = true;
                x.transform.Find("Debug").GetComponent<SpriteRenderer>().color = x == currentlyTrackedMallow ? Color.red : Color.white;
            }
            */
        }
        heightText.text = currHeight.ToString();
        if(currentlyTrackedMallow)
            heightCanvas.transform.position = new Vector2(0, currHeightRaw + .6f); // Adds half of height of marshmallow to make line on top of mallow
        remainingText.text = (marshmallowCount - marshmallows.Count).ToString() + " left";
        if (holding)
        {
            marshmallow_instance.transform.position = transform.position;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetMouseButton(0))
                    ControlMarshmallow(cam.ScreenToWorldPoint(Input.mousePosition));
                else if (Input.GetMouseButtonUp(0))
                    DropMarshmallow(cam.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        else
        {
            if (marshmallows.Count >= marshmallowCount)
            {
                //TODO: Error indicator that out of marshmallows
            }
            else
                SpawnHoldMallow();
        }
    }

    void ControlMarshmallow(Vector2 pos)
    {
        if (holding)
            transform.position = marshmallow_instance.transform.position = pos;
    }

    void DropMarshmallow(Vector2 pos) // Drops marshmallow at pos
    {
        if (canDrop && holding && pos.y >= cam.transform.position.y + 3)
        {
            marshmallow_instance.SetActive(true);
            transform.position = marshmallow_instance.transform.position = pos;
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
            StartCoroutine("NextDropDelay");
        }
    }

    void SpawnHoldMallow()
    {
        if (canDrop)
        {
            marshmallow_instance = Instantiate(marshmallow_prefab, transform.position, Quaternion.identity);
            marshmallow_instance.GetComponent<Rigidbody2D>().freezeRotation = true;
            marshmallow_instance.name = marshmallows != null ? marshmallows.Count.ToString() : "0";

            marshmallow_instance.SetActive(false);
            var y = marshmallow_instance.GetComponent<StickToOther>();
            y.willBurn = false;
            y.stuckEvent.AddListener(
                (x) =>
                {
                    if(!y.landed) // first time landing
                    {
                        currentlyTrackedMallow = x.gameObject;
                        if (marshmallows.Count == 1 || forceStopCameraTrack)
                            StartCoroutine("CamTrackMarshmallow");
                    }
                });
            y.unstuckEvent.AddListener(
                (x) =>
                {
                    //StartCoroutine("CamTrackMarshmallow", x);
                });

            holding = true;
        }
    }

    IEnumerator CamTrackMarshmallow() // Tracks the currentlyTrackedMallow
    {
        if (!movingCamera && currentlyTrackedMallow != null)
        {
            movingCamera = true;
            forceStopCameraTrack = false;
            float targetPosY;
            do
            {
                targetPosY = (float)System.Math.Round(Mathf.Clamp(currentlyTrackedMallow.transform.position.y + 1, 0, 999), 2);
                cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(0, targetPosY, -10), Time.deltaTime * 3);
                yield return new WaitForFixedUpdate();
            } while (!forceStopCameraTrack);
            movingCamera = false;
        }
    }

    IEnumerator NextDropDelay() // Delay for consecutive tapping
    {
        canDrop = false;
        yield return new WaitForSeconds(.5f);
        canDrop = true;
    }

    /* NOTES
     * Need to drop from height above 
     * 
     * 
     * 
     * 
     *
     */
        }
