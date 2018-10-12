using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCodeMega : MonoBehaviour {

    public GameObject Boss;

    public AudioClip BossDead;

    public AudioSource source;

	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Boss == null)
        {
            source.PlayOneShot(BossDead, 0.7F);
        }
    }
}
