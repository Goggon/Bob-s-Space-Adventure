using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricode : MonoBehaviour {

    public int health = 3;
    public GameObject Barricade;

	
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

    //private void OnTriggerEnter2D(Collider2D Trig)
    //{
    //    if (Trig.gameObject.tag == "shot")
    //    {
    //        if (health > 1)
    //        {
    //            health = health - 1;
    //        }
    //        else if (health == 1)
    //        {
    //            Destroy(Barricade);
    //        }
    //    }
    //}
}
