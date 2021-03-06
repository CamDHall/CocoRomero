﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType { Horiztonal, Vertical }
public class Platform : MonoBehaviour {

    public Color safeColor;
    public Color moderateColor;
    public Color highColor;

    public int safeHeight;
    public int moderateHeight;
    public int highHeight;

    public int startingDir;

    public float moveSpeed;
    public float directionTimerLength;

    private float directionTimer;

    private Color currentTargetColor;
    Vector3 horizontalDir;
    Vector3 verticalDir;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    public ObstacleType obstacleType;
    delegate void Move();
    Move platformMove;

	void Start () {
        horizontalDir = transform.right * startingDir;
        verticalDir = transform.up * startingDir;
        rb = GetComponentInChildren<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        rb.gravityScale = 0;
        rb.mass = 999999999;

        if (obstacleType == ObstacleType.Horiztonal)
        {
            platformMove = MoveHoritonzal;
        } else if(obstacleType == ObstacleType.Vertical)
        {
            platformMove = MoveVertical;
        }
	}

    void Update()
    {
        if (directionTimer < Time.timeSinceLevelLoad)
        {
            horizontalDir *= -1;
            verticalDir *= -1;
            directionTimer = Time.timeSinceLevelLoad + directionTimerLength;
        }

        if (ObstacleType.Vertical == obstacleType)
        {
            float highestPoint = spriteRenderer.size.y
        + transform.localPosition.y;

            if (highestPoint > highHeight)
            {
                currentTargetColor = highColor;
            }
            else if (highestPoint > safeHeight)
            {
                currentTargetColor = moderateColor;
            }
            else
            {
                currentTargetColor = safeColor;
            }

            spriteRenderer.color = Color.Lerp(spriteRenderer.color, currentTargetColor, Time.deltaTime * 10);
        }
    }

    void FixedUpdate () {
        platformMove();
	}

    public void MoveHoritonzal()
    {
        rb.MovePosition(rb.position + (Vector2)(horizontalDir * moveSpeed * Time.deltaTime));
    }

    public void MoveVertical()
    {
        transform.position += verticalDir * moveSpeed * Time.deltaTime;
    }
}
