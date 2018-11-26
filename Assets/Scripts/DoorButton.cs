using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour {

    public Animator door;
    Animation anim;

	void Start () {

	}
	
	void Update () {
	}

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.tag == "Player")
        {
            door.SetTrigger("open");
        }
    }
}
