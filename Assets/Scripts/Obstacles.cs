using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType {  Horiztonal, Vertical }
public class Obstacles : MonoBehaviour {

    public float moveSpeed;
    public float directionTimerLength;

    private float directionTimer;

    Vector3 horizontalDir;
    Vector3 verticalDir;

	public void Start () {
        horizontalDir = transform.right;
        verticalDir = transform.up;
	}
	
	void Update () {
		if(directionTimer < Time.timeSinceLevelLoad)
        {
            horizontalDir *= -1;
            verticalDir *= -1;
            directionTimer = Time.timeSinceLevelLoad + directionTimerLength;
        }
	}

    public void MoveHoritonzal()
    {
        transform.position += horizontalDir * moveSpeed * Time.deltaTime;
    }

    public void MoveVertical()
    {
        transform.position += verticalDir * moveSpeed * Time.deltaTime;
    }

    public void Swing()
    {

    }
}
