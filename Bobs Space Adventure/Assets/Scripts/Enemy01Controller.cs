using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Controller : MonoBehaviour {

    private Rigidbody2D rbxd;

    public float movespeed;

    Vector2 Move;

    private int isthissatart;

    // Use this for initialization
    void Start () {
        rbxd = GetComponent<Rigidbody2D>();
        isthissatart = 1;
        Move = new Vector2(0, movespeed);

        

        Mover();
    }
	

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            Mover();
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
