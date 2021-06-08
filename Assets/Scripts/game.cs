using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class game : MonoBehaviour
{

    public GameObject stickTop;
    public GameObject stickBottom;
    public GameObject teammember;

    public GameObject borderTop;
    public GameObject borderBottom;

    public GameObject btnSingle;
    public GameObject btnMulti;

    public GameObject bestSingle;
    public GameObject bestMulti;

    public TextMesh tmScore;

    private float acceleration = -1.05f;

    private Rigidbody2D rigid;

    private int mode = 0;
    private int counter = 0;

    private bool Collision = false;
    private bool inGame = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();


        stickTop.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.9f, 10));
        stickBottom.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, 10));
        teammember.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 11));

        borderTop.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, 10));
        borderBottom.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, 10));

        transform.localScale = new Vector3(10, 10, 10);

        btnSingle.GetComponent<Button>().onClick.AddListener(delegate { TaskOnClick(0); });
        btnMulti.GetComponent<Button>().onClick.AddListener(delegate { TaskOnClick(1); });

        bestSingle.GetComponent<Text>().text = "Best: " + PlayerPrefs.GetInt("single", 0).ToString();
        bestMulti.GetComponent<Text>().text = "Best: " + PlayerPrefs.GetInt("multi", 0).ToString();

        StartCoroutine(FadeInCanvas());
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && inGame)
        {
            if (Input.mousePosition.y > Screen.height / 2)
            {
                // Top clicked
                if (Collision && rigid.velocity.y > 0)
                {
                    Collision = false;
                    rigid.velocity = rigid.velocity * acceleration;
                    counter++;
                    tmScore.text = counter.ToString();
                }
            }
            else
            {
                // Bottom clicked
                if (Collision && rigid.velocity.y < 0 && transform.position.y < 0)
                {
                    Collision = false;
                    rigid.velocity = rigid.velocity * acceleration;
                    counter++;
                    tmScore.text = counter.ToString();
                }
            }
        }
    }

    void startGame()
    {
        rigid.velocity = new Vector3(0, -3, 0);
        inGame = true;
        Collision = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (inGame)
        {
            Collision = true;
            if (col.tag == "top" && mode == 0)
            {
                rigid.velocity = rigid.velocity * acceleration;
                counter++;
                tmScore.text = counter.ToString();
            }
            else if (col.tag == "borderbottom" || col.tag == "bordertop")
            {
                rigid.velocity = new Vector3(0, 0, 0);
                if (mode == 0 && counter > PlayerPrefs.GetInt("single", 0))
                {
                    PlayerPrefs.SetInt("single", counter);
                }
                else if (mode == 1 && counter > PlayerPrefs.GetInt("multi", 0))
                {
                    PlayerPrefs.SetInt("multi", counter);
                }

                inGame = false;
                counter = 0;

                StartCoroutine(FadeOut());
            }
        }

    }

    void OnTriggerExit2D(Collider2D col)
    {
        Collision = false;
    }

    void TaskOnClick(int m)
    {
        mode = m;
        if (mode == 0)
        {
            teammember.SetActive(true);
            switch (Random.Range(0, 3))
            {
                case 0:
                    teammember.GetComponent<TextMesh>().text = "Your teammate:\nBill the bot";
                    break;
                case 1:
                    teammember.GetComponent<TextMesh>().text = "Your teammate:\nRoy the robot";
                    break;
                case 2:
                    teammember.GetComponent<TextMesh>().text = "Your teammate:\nTim the teammate";
                    break;
                case 3:
                    teammember.GetComponent<TextMesh>().text = "Your teammate:\nPaul the pong-player";
                    break;
            }
        }
        else
        {
            teammember.SetActive(false);
        }

        StartCoroutine(FadeOutCanvas());
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timedStart = 1f;
        float timed = timedStart;

        while (timed > 0.0f)
        {
            timed -= Time.deltaTime;
            transform.localScale = Vector3.Lerp(new Vector3(0.2f, 0.2f, 0.2f), new Vector3(10, 10, 10), timed / timedStart);
            yield return null;
        }

        Invoke("startGame", 1);

    }

    IEnumerator FadeOut()
    {
        float timedStart = 1f;
        float timed = timedStart;

        while (timed > 0.0f)
        {
            timed -= Time.deltaTime;
            transform.localScale = Vector3.Lerp(new Vector3(10f, 10f, 10f), new Vector3(0.2f, 0.2f, 0.2f), timed / timedStart);
            yield return null;
        }

        btnSingle.SetActive(true);
        btnMulti.SetActive(true);
        bestSingle.SetActive(true);
        bestMulti.SetActive(true);
        StartCoroutine(FadeInCanvas());
        bestSingle.GetComponent<Text>().text = "Best: " + PlayerPrefs.GetInt("single", 0).ToString();
        bestMulti.GetComponent<Text>().text = "Best: " + PlayerPrefs.GetInt("multi", 0).ToString();
        transform.position = new Vector3(0, 0, 0);

        tmScore.text = "0";

    }

    IEnumerator FadeInCanvas()
    {
        bestSingle.GetComponent<Text>().color = new Color32(21, 161, 86, 0);
        float fZwischenergebnis = 1;
        while (bestSingle.GetComponent<Text>().color.a < 1)
        {
            fZwischenergebnis -= Time.deltaTime * 2;
            bestSingle.GetComponent<Text>().color = new Color32(239, 96, 96, (byte)Mathf.Lerp(0, 255, 1 - fZwischenergebnis));
            bestMulti.GetComponent<Text>().color = new Color32(239, 96, 96, (byte)Mathf.Lerp(0, 255, 1 - fZwischenergebnis));
            btnSingle.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, 1 - fZwischenergebnis));
            btnMulti.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, 1 - fZwischenergebnis));
            yield return null;
        }
    }

    IEnumerator FadeOutCanvas()
    {
        float fZwischenergebnis = 1;
        while (bestSingle.GetComponent<Text>().color.a > 0)
        {
            fZwischenergebnis -= Time.deltaTime * 2;
            bestSingle.GetComponent<Text>().color = new Color32(239, 96, 96, (byte)Mathf.Lerp(0, 255, bestSingle.GetComponent<Text>().color.a - fZwischenergebnis));
            bestMulti.GetComponent<Text>().color = new Color32(239, 96, 96, (byte)Mathf.Lerp(0, 255, bestSingle.GetComponent<Text>().color.a - fZwischenergebnis));
            btnSingle.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, bestSingle.GetComponent<Text>().color.a - fZwischenergebnis));
            btnMulti.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, bestSingle.GetComponent<Text>().color.a - fZwischenergebnis));
            yield return null;
        }

        btnSingle.SetActive(false);
        btnMulti.SetActive(false);
        bestSingle.SetActive(false);
        bestMulti.SetActive(false);
    }
}
