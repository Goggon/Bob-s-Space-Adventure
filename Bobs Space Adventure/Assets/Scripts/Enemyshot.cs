using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyshot : MonoBehaviour {

    private Rigidbody2D rb2d;

    public float speed;

    public GameObject shot;



    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.velocity = -transform.right * speed;
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Player")
        {
            Destroy(shot);
            Destroy(obj.gameObject);
        }
        else if (obj.gameObject.tag == "Enemy1")
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), GetComponent<Collider>());
        }
        else if (obj.gameObject.tag == "walls")
        {
            Destroy(shot);
        }

    }
}
