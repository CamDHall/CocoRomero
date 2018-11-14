using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour {

    public float flameLength;
    public float duration;

    float flameTimer;
    ParticleSystem ps;

	void Start () {
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Stop();
        var main = ps.main;
        main.duration = duration;
	}
	

	void Update () {
		if(Time.timeSinceLevelLoad > flameTimer)
        {
            ps.Play();
            flameTimer = Time.timeSinceLevelLoad + flameLength;
        }
	}
}
