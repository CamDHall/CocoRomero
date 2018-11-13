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
    Rigidbody2D rb;

	public void Start () {
        horizontalDir = transform.right;
        verticalDir = transform.up;
        rb = GetComponent<Rigidbody2D>();
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
        rb.MovePosition(rb.position + (Vector2)(horizontalDir * moveSpeed * Time.deltaTime));
    }

    public void MoveVertical()
    {
        transform.position += verticalDir * moveSpeed * Time.deltaTime;
    }

    public void Swing()
    {

    }
}
