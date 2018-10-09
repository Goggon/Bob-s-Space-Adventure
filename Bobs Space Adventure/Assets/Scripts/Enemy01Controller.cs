using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Controller : MonoBehaviour {

    private Rigidbody2D rbxd;

    public float movespeed;

    Vector2 Move;

    private int isthissatart;

    public Transform pos;
    public GameObject enemyweapon;

    public float cooldown;
    public float cooldowntimer;

    // Use this for initialization
    void Start () {
        rbxd = GetComponent<Rigidbody2D>();
        isthissatart = 1;
        Move = new Vector2(0, movespeed);

        cooldown = Random.Range(3, 20);

        cooldowntimer = cooldown;

        Mover();
    }

    private void Update()
    {
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
            Instantiate(enemyweapon, pos.position, pos.rotation);
            cooldowntimer = Random.Range(7, 20);
        }
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            Mover();
        }
    }

    private void FixedUpdate()
    {


        
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
