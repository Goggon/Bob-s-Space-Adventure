using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyshot : MonoBehaviour {

    private Rigidbody2D rb2d;
    public GameObject shooty;

    public float speed;

    public GameObject shot;

    public AudioClip Enemypew;

    public AudioSource source;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.velocity = -transform.right * speed;
        source.PlayOneShot(Enemypew, 0.7F);
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            Destroy(obj.gameObject);
        }
        else if (obj.gameObject.tag == "Walls")
        {
            Destroy(this.gameObject);
        }
    }
}
