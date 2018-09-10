using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MobileHelper : MonoBehaviour
{
    public class TouchEvent : UnityEvent<Vector2>
    {
    }

    public static MobileHelper ins;
    public TouchEvent tapEvent = new TouchEvent();
    public TouchEvent holdingEvent = new TouchEvent();
    public TouchEvent holdReleaseEvent = new TouchEvent();
    public UnityEvent panEvent = new UnityEvent();

    private float holdTime = 0.1f;
    private float acumTime = 0;
    bool held = false;
    private float speedFactor = .005f;
    Vector3 panMove = Vector3.zero;
    public bool canControlCamera = true;
    private Vector3 currentPosition;
    private Vector3 deltaPosition;
    private Vector3 lastPosition;
    private bool held_two = false;

    // Use this for initialization
    void Start()
    {
        ins = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (canControlCamera)
            HandleCamera();
    }

    void HandleCamera()
    {
        currentPosition = Input.mousePosition;
        deltaPosition = currentPosition - lastPosition;
        lastPosition = currentPosition;
        if (Input.touchCount == 1 && !held_two)
        {
            var x = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).rawPosition);
            acumTime += Input.GetTouch(0).deltaTime;

            if (acumTime >= holdTime)
            {
                held = true;
                print(x);
                holdingEvent.Invoke(x);
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (held)
                    holdReleaseEvent.Invoke(x);
                acumTime = 0;
                tapEvent.Invoke(x);
            }
        }
        else if (Input.touchCount == 2)
        {
            held_two = true;
            panEvent.Invoke();
            Vector2 totalMove = Vector2.zero;
            foreach (Touch thisTouch in Input.touches)
            {
                totalMove += thisTouch.deltaPosition;
            }
            /* calculate average movement for each touch */
            panMove = totalMove / 2;
            transform.position = transform.position - new Vector3(0, panMove.y) * speedFactor;
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
                StartCoroutine("DelayTwoToOneTouch");
        } else if (Input.GetKey(KeyCode.E)) // MOUSE DEBUG for dragging
        {
            panEvent.Invoke();
            panMove = deltaPosition / 2;
            transform.position = transform.position - new Vector3(0, panMove.y) * speedFactor;
        }

        
    }

    IEnumerator DelayTwoToOneTouch() // Delay so that if you release two fingers not simultaneously after panning, it won't activate tap
    {
        yield return new WaitForSeconds(.2f);
        if (Input.touchCount != 2)
            held_two = false;
    }
}
