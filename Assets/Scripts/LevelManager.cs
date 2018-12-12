using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public GameObject introText;

    void Start () {
        introText.SetActive(true);
	}
	
	void Update () {
		if(Input.GetKey(KeyCode.Space))
        {
            introText.SetActive(false);
        }
	}
}
