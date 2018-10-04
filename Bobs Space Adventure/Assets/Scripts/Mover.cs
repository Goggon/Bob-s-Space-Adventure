using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mover : MonoBehaviour {

    private Rigidbody2D rb2d;

    public float speed;

    public GameObject shot;

    public Text Score;
    private int point = 0;

    // Use this for initialization
    void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.velocity = transform.right * speed;

        Score.text = "Score: " + point;
    }


    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            Destroy(shot);
        }
        else if (obj.gameObject.tag == "Enemy1")
        {
            Destroy(shot);
            Destroy(obj.gameObject);
            point = point + 1;
            Score.text = "Score: " + point;
            
            
            
        }
    }

}
