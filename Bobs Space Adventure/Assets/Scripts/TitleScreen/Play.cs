﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour {

    public string lvl1;

    public void startogamo()
    {
        SceneManager.LoadScene(lvl1);
    }
}