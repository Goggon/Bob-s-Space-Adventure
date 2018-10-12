using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.


    private Vector3 Spawnpos = new Vector3(-7.42F, 3.04F, 0);
    public float MoveSpeed;

    public bool invert = true;
    public bool begin = true;

    public GameObject Player;

    public GameObject Weapon;
    public Transform WeaponSpawn;

    public Text guiscore;
    private int score;
    public int wincondition;

    public Text Combonr;
    private int combonumero;

    public Text Wintext;
    public Text WinUnder;

    private int enemynumber;

    public bool nextlvl;
    public bool final;

    public string lvl2;
    public string lvl3;

    // Use this for initialization
    void Start () {
        this.transform.position = (Spawnpos);

        rb2d = GetComponent<Rigidbody2D>();

        GameObject[] enemycount = GameObject.FindGameObjectsWithTag("Enemy1");
        enemynumber = enemycount.Length;
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        guiscore.text = "Score: " + score;
        wincondition = score;
    }

    public void comboboi(int combonew)
    {
        if (combonew == 0)
        {
            combonumero = combonew;
            Combonr.text = combonumero + "x";
        }
        else
        {
            combonumero += combonew;
            Combonr.text = combonumero + "x";
        }
        
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space") && nextlvl == true && final == true)
        {
            SceneManager.LoadScene(lvl3);
            Time.timeScale = 1;
        }
        else if (Input.GetKeyDown("space") && nextlvl == true && final == false)
        {
            SceneManager.LoadScene(lvl2);
            Time.timeScale = 1;
            nextlvl = false;
            final = true;
        }
        else if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0))
        {
            Vector2 move = new Vector2(0, MoveSpeed);

            if (begin == true)
            {
                rb2d.AddForce(move * -1);
                invert = false;
                begin = false;
            }
            else if (invert == false)
            {
                rb2d.AddForce(move * 2);
                invert = true;
            }
            else if (invert == true)
            {
                rb2d.AddForce(move * -2);
                invert = false;
            }
            Instantiate(Weapon, WeaponSpawn.position, WeaponSpawn.rotation);
        }


	}

    private void FixedUpdate()
    {
        if (wincondition == enemynumber)
        {
            nextlvl = true;
            Wintext.GetComponent<Text>().enabled = true;
            WinUnder.GetComponent<Text>().enabled = true;
            Time.timeScale = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            Destroy(Player);
        }
    }
}
