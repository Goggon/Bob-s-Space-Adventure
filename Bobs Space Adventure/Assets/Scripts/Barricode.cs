﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricode : MonoBehaviour {

    private int health;
    public GameObject Barricade;


	// Use this for initialization
	void Start ()
    {
        health = 3;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		
	}

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "shot")
        {
            if (health > 1)
            {
                health = health - 1;
            }
            else if (health == 1)
            {
                Destroy(Barricade);
            }
        }

    }
}