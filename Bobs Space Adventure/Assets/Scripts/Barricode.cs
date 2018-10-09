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


    // Update is called once per frame
    void FixedUpdate()
    {

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
                    break;
                case 2:
                    this.GetComponent<SpriteRenderer>().sprite = twodmg;
                    health -= 1;
                    break;
                case 1:
                    Destroy(Barricade);
                    break;
            }
        }
    }
}
