using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheckMarshmallowDeath : MonoBehaviour
{

    public static CheckMarshmallowDeath ins;

    public int lives = 5;
    public StickToOther stick;
    private Canvas endScreen;

    private void Awake()
    {
        ins = this;
    }

    // Use this for initialization
    void Start()
    {
        endScreen = Camera.main.transform.Find("EndScreen").GetComponent<Canvas>();
        endScreen.transform.Find("RestartBtn").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("MainGame"));
        endScreen.transform.Find("MenuBtn").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("StartMenu"));
    }

    // Update is called once per frame
    void Update()
    {
        if (lives <= 0)
        {
            endScreen.transform.Find("CookedTxt").GetComponent<Text>().text = StickToOther.cooked.ToString();
            endScreen.enabled = true;
            GameObject.Find("Crane").GetComponent<Crane>().enabled = false;
            foreach (GameObject x in FindObjectsOfType<GameObject>())
                if (x.GetComponent<StickToOther>() != null)
                    Destroy(x);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //only "live" objects will trip this
        if (collision.gameObject.tag == "Sticky")
        {
            lives--;
            Destroy(collision.gameObject);
        }
    }
}