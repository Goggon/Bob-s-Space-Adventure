using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Controller : MonoBehaviour {

    private Rigidbody2D rb2d;

    public float movespeed;

    Vector2 Move;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        Vector2 Move = new Vector2(0, movespeed);
        rb2d.velocity = (Move);
    }
	
	// Update is called once per frame
	void Update () {
        rb2d.velocity = (Move);
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            rb2d.velocity = (Move * -1);
            Debug.Log("help");
        }
    }
}
