using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour {

    public float speed;

    float lastZ;
    float newZ;

    private void Start()
    {
        lastZ = transform.parent.localRotation.z;
    }

    void Update () {

        if (lastZ > transform.parent.localRotation.z)
        {
            newZ = 1.5f * speed * Time.deltaTime;
        } else
        {
            newZ = speed * Time.deltaTime;
            //newZ = (speed * Time.deltaTime) + (transform.parent.localRotation.z - lastZ);
        }

        transform.Rotate(new Vector3(0, 0, newZ));

        lastZ = transform.parent.localRotation.z;
	}
}
