using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllableCrane : MonoBehaviour {
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
    float gamemodeMaxHeight = 0;
    private Text maxHeightText, remainingText;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        SpawnHoldMallow();
        holding = true;
        direction = Vector2.right;
        if (usingAndroid) speed /= 2;
        marshmallows = new List<GameObject>();
        MobileHelper.ins.tapEvent.AddListener((x) => DropMarshmallow(x));
        MobileHelper.ins.holdReleaseEvent.AddListener((x) => DropMarshmallow(x));
        MobileHelper.ins.holdingEvent.AddListener((x) => ControlMarshmallow(x));
        MobileHelper.ins.panEvent.AddListener(() => forceStopCameraTrack = true);
        maxHeightText = cam.transform.Find("Canvas").transform.Find("MaxHeight").GetComponent<Text>();
        remainingText = cam.transform.Find("Canvas").transform.Find("Remaining").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        maxHeightText.text = gamemodeMaxHeight.ToString();
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
        if(holding)
            transform.position = marshmallow_instance.transform.position = pos;
    }

    void DropMarshmallow(Vector2 pos) // Drops marshmallow at pos
    {
        if (holding && pos.y >= cam.transform.position.y + 3)
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
            StartCoroutine("CamTrackMarshmallow");
            StartCoroutine("NextDropDelay");
        }
    }

    void SpawnHoldMallow()
    {
        if (canDrop)
        {
            marshmallow_instance = Instantiate(marshmallow_prefab, transform.position, Quaternion.identity);
            marshmallow_instance.GetComponent<Rigidbody2D>().freezeRotation = true;

            marshmallow_instance.SetActive(false);
            marshmallow_instance.GetComponent<StickToOther>().stuckEvent.AddListener(
                (x) =>
                {
                    if (Math.Round((x.position.y + 4) * 10, 2) >= gamemodeMaxHeight)
                    {
                        gamemodeMaxHeight = (float) Math.Round((x.position.y + 4) * 10, 2);
                    }
                }); // while each mallow is stuck, update highest one height to max height

            holding = true;
        }
    }

    IEnumerator CamTrackMarshmallow()
    {
        if (marshmallows.Count > 0 && !movingCamera)
        {
            movingCamera = true;
            forceStopCameraTrack = false;
            float targetPosY;
            do
            {
                targetPosY = (float)System.Math.Round(Mathf.Clamp(marshmallows[marshmallows.Count - 1].transform.position.y - 2, 0, 999), 2);
                cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(0, targetPosY, -10), Time.deltaTime * 3);
                yield return new WaitForFixedUpdate();
            } while (cam.transform.position.y != targetPosY && !forceStopCameraTrack);
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
