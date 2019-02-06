using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public bool forceStopCameraTrack = true;
    private bool canDrop = true, canHold = true;
    int currHeight = 0;
    float currHeightRaw = 0;
    private GameObject heightCanvas, dropZone, currentlyTrackedMallow;
    private Canvas endScreen, pauseCanvas;
    private Text heightText, remainingText;
    private Image canDropIndicator;
    Color greenInd = new Color(0, 1, 11 / 255, 32f / 255), redInd = new Color(1, 0, 29 / 255, 32f / 255);

    bool isPaused = false;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        SpawnHoldMallow();
        if (usingAndroid) speed /= 2;
        marshmallows = new List<GameObject>();
        MobileHelper.ins.tapEvent.AddListener((x) => DropMarshmallow(x));
        MobileHelper.ins.holdReleaseEvent.AddListener((x) => DropMarshmallow(x));
        MobileHelper.ins.holdingEvent.AddListener((x) => ControlMarshmallow(x));
        MobileHelper.ins.panEvent.AddListener(() =>
        {
            forceStopCameraTrack = true;
            cam.transform.position = new Vector3(0, Mathf.Clamp(cam.transform.position.y, 0, 999), -10);
        });
        heightCanvas = GameObject.Find("HeightCanvas");
        heightText = heightCanvas.transform.Find("Height").GetComponent<Text>();
        dropZone = cam.transform.Find("DropZone").gameObject;
        remainingText = dropZone.transform.Find("Remaining").GetComponent<Text>();
        canDropIndicator = dropZone.transform.Find("CanDrop").GetComponent<Image>();
        endScreen = cam.transform.Find("EndScreen").GetComponent<Canvas>();
        endScreen.transform.Find("RestartBtn").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("TowerBuild"));
        endScreen.transform.Find("MenuBtn").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("StartMenu"));

        pauseCanvas = cam.transform.Find("PauseCanvas").GetComponent<Canvas>();
        pauseCanvas.transform.Find("RestartBtn").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("TowerBuild"));
        pauseCanvas.transform.Find("MenuBtn").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("StartMenu"));
        pauseCanvas.transform.Find("ResumeBtn").GetComponent<Button>().onClick.AddListener(() => PauseGame());
        pauseCanvas.transform.Find("PauseBtn").GetComponent<Button>().onClick.AddListener(() => PauseGame());
    }

    // Update is called once per frame
    void Update()
    {
        currHeight = 0;
        foreach (GameObject x in marshmallows)
        {
            if (x.GetComponent<StickToOther>().landed)
            {
                var y = Math.Round((x.transform.position.y + 6) * 10);
                if (y > currHeight)
                {
                    currHeightRaw = x.transform.position.y;
                    currHeight = (int)y;
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
        if (currentlyTrackedMallow)
        {
            heightCanvas.transform.position = new Vector2(0, currHeightRaw + .6f); // Adds half of height of marshmallow to make line on top of mallow
            canDrop = cam.transform.position.y >= currentlyTrackedMallow.transform.position.y + .5f;
            canDropIndicator.color = canDrop ? greenInd : redInd;
        }
        remainingText.text = (marshmallowCount - marshmallows.Count).ToString() + " left";
        if (holding)
        {
            marshmallow_instance.transform.position = transform.position;
            if (Input.GetMouseButton(0))
                ControlMarshmallow(cam.ScreenToWorldPoint(Input.mousePosition));
            else if (Input.GetMouseButtonUp(0))
                DropMarshmallow(cam.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
            if (marshmallows.Count >= marshmallowCount)
            {
                StartCoroutine("CheckFinishHeight");
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
        if (pos.y >= cam.transform.position.y + 3)
        {
            if (canDrop && holding)
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
                StartCoroutine("NextHoldDelay");
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0);
            if (hit)
            {
                if (hit.transform.GetComponentInParent<StickToOther>() != null)
                {
                    var m = hit.transform.GetComponentInParent<StickToOther>().gameObject;
                    marshmallows.Remove(m);
                    Destroy(m);
                }
            }
        }
    }

    void SpawnHoldMallow()
    {
        if (canHold)
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
                    if (!y.landed) // first time landing
                    {
                        currentlyTrackedMallow = x.gameObject;
                        if (forceStopCameraTrack)
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
            } while (!forceStopCameraTrack && currentlyTrackedMallow != null);
            movingCamera = false;
        }
    }

    IEnumerator NextHoldDelay() // Delay for consecutive tapping
    {
        canHold = false;
        yield return new WaitForSeconds(.5f);
        canHold = true;
    }

    IEnumerator CheckFinishHeight()
    {
        float x = currHeight;
        yield return new WaitForSeconds(3);
        if (currHeight == x)
        {
            endScreen.transform.Find("FinalHeightTxt").GetComponent<Text>().text = x.ToString();
            endScreen.enabled = true;
            foreach (GameObject m in marshmallows)
            {
                m.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            StopAllCoroutines();
        }
    }

    void PauseGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            pauseCanvas.enabled = false;
        }
        else
        {
            pauseCanvas.enabled = true;
            Time.timeScale = 0f;
            isPaused = true;
        }


    }
}
