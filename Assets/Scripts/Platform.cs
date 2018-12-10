using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType { Horiztonal, Vertical }
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Platform : MonoBehaviour {

    public Color safeColor;
    public Color moderateColor;
    public Color highColor;

    public int safeHeight;
    public int moderateHeight;
    public int highHeight;

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
        horizontalDir = transform.right;
        verticalDir = transform.up;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
