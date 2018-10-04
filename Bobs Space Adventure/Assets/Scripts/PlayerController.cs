﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.


    private Vector3 Spawnpos = new Vector3(-9, 3.5F, 0);
    public float MoveSpeed;

    public bool invert = true;
    public bool begin = true;

    public GameObject Player;

    public GameObject Weapon;
    public Transform WeaponSpawn;

	// Use this for initialization
	void Start () {
        this.transform.position = (Spawnpos);

        rb2d = GetComponent<Rigidbody2D>();       
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space") || Input.GetMouseButtonDown(0))
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

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            Destroy(Player);
        }
    }
}
