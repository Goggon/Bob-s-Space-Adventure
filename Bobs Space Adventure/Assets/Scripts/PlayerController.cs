using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour {

    
    private Vector3 Spawnpos = new Vector3(-9, 3.5F, 0);

	// Use this for initialization
	void Start () {
        this.transform.position = (Spawnpos);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
