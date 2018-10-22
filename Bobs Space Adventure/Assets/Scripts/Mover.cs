using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mover : MonoBehaviour {

    private Rigidbody2D rb2d;

    public float speed;

    public GameObject shot;

    public int scoreValue;
    private PlayerController gameController;

    
    private int combo;


    public AudioClip pew;

    public AudioSource source;

    // Use this for initialization

    void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.velocity = transform.right * speed;
        source.PlayOneShot(pew, 0.7F);
        GameObject gameControllerObject = GameObject.FindWithTag("Player");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<PlayerController>();
        }
        else if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }


    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Walls")
        {
            Destroy(shot);
            combo = 0;
            gameController.comboboi(combo);
        }
        else if (obj.gameObject.tag == "Enemy1")
        {
            scoreValue = 1;
            Destroy(shot);
            Destroy(obj.gameObject);
            gameController.AddScore(scoreValue);
            combo = 1;
            gameController.comboboi(combo);
        }
        
    }

}
