using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricode : MonoBehaviour
{

    public int health = 3;
    public GameObject Barricade;

    public Sprite nodmg;
    public Sprite onedmg;
    public Sprite twodmg;

    public AudioClip Barrierdmg;

    public AudioSource source;


    void Start()
    {
        //source = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "shot")
        {
            switch (health)
            {
                case 3:
                    this.GetComponent<SpriteRenderer>().sprite = onedmg;
                    health -= 1;
                    source.PlayOneShot(Barrierdmg, 0.7F);
                    break;
                case 2:
                    this.GetComponent<SpriteRenderer>().sprite = twodmg;
                    health -= 1;
                    source.PlayOneShot(Barrierdmg, 0.7F);
                    break;
                case 1:
                    Destroy(Barricade);
                    source.PlayOneShot(Barrierdmg, 0.7F);
                    break;
            }
        }
    }
}
