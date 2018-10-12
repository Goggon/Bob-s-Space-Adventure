using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CamCode : MonoBehaviour {

    public GameObject player;
    private bool reset;
    public Text retry;

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (player == null)
        {
            retry.text = "press R to retry";
            reset = true;

        }
        if (Input.GetKeyDown(KeyCode.R) && reset == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
