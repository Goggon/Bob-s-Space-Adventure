using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {

    private Rigidbody2D rbxd;

    public float movespeed;

    Vector2 Move;

    private int isthissatart;

    public Transform pos;
    public Transform pos2;
    public GameObject enemyweapon;

    public float cooldown;
    public float cooldowntimer;

    public int weaponchoose;

    public int bosshp;


    public int scoreValue;
    private PlayerController gameController;


    private int combo;

    // Use this for initialization
    void Start () {
        rbxd = GetComponent<Rigidbody2D>();
        isthissatart = 1;
        Move = new Vector2(0, movespeed);
        Mover();

        cooldown = Random.Range(0, 2);

        cooldowntimer = cooldown;

        GameObject gameControllerObject = GameObject.FindWithTag("Player");
        gameController = gameControllerObject.GetComponent<PlayerController>();
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            Mover();
        }
        if (obj.gameObject.tag == "shot")
        {
            if (bosshp > 1)
            {
                bosshp -= 1;
                Destroy(obj.gameObject);
            }
            else if (bosshp == 1)
            {
                Destroy(this.gameObject);
                Destroy(obj.gameObject);
            }

            scoreValue = 1;
            gameController.AddScore(scoreValue);
            combo = 1;
            gameController.comboboi(combo);
        }
    }

    // Update is called once per frame
    private void Update () {
        if (cooldowntimer > 0)
        {
            cooldowntimer -= Time.deltaTime;
        }

        if (cooldowntimer < 0)
        {
            cooldowntimer = 0;
        }

        if (cooldowntimer == 0)
        {
            weaponchoose = Random.Range(1, 3);
            
            switch(weaponchoose)
            {
                case 1:
                    Instantiate(enemyweapon, pos2.position, pos2.rotation);
                    break;
                case 2 | 3:
                    Instantiate(enemyweapon, pos.position, pos.rotation);
                    break;
            }

            if(weaponchoose == 1)
            {
                Instantiate(enemyweapon, pos2.position, pos2.rotation);
            }
            else if (weaponchoose == 2)
            {
                Instantiate(enemyweapon, pos.position, pos.rotation);
            }
            cooldowntimer = Random.Range(0, 2);
        }
    }

    void Mover()
    {
        switch (isthissatart)
        {
            case 1:
                rbxd.velocity = (Move * -1);
                isthissatart = 2;
                break;
            case 2:
                rbxd.velocity = (Move * 1);
                isthissatart = 3;
                break;
            case 3:
                rbxd.velocity = (Move * -1);
                isthissatart = 2;
                break;
        }
    }
}
