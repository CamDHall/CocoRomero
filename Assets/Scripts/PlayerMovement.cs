using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour {

    public Vector2 anchorOffset;
    public float horizontalSpeed;
    public float _horizontalCooldownAmount;
    public float forceAmount;
    public int maxAngularVel;
    public int maxImpactVel;

    float horizontalCooldownTimer = 0;
    float signVal;
    float breakDelayTimer;
    int hitIndex;

    Vector2 playerInput;
    Vector3 additivePos;
    Rigidbody2D rb;
    List<Transform> pieces;


    void Start () {
        playerInput = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
        pieces = GetComponentsInChildren<Transform>().ToList();
        pieces.Remove(transform);
        signVal = 0;

        pieces[0].position = (Vector2)pieces[1].position - anchorOffset;
        pieces[pieces.Count - 1].position = (Vector2)pieces[pieces.Count - 2].position + anchorOffset;
	}
	
	void Update () {
        if (horizontalCooldownTimer < Time.timeSinceLevelLoad)
        {
            playerInput.x = Input.GetKeyDown(KeyCode.D) ? 1 : Input.GetKeyDown(KeyCode.A) ? -1 : 0;
            if (playerInput.x != 0)
            {
                horizontalCooldownTimer = Time.timeSinceLevelLoad + (_horizontalCooldownAmount * Time.deltaTime);
            }
        } else
        {
            playerInput.x = 0;
        }
        playerInput.y = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        additivePos = Vector3.zero;

        if (playerInput.y != 0)
        {
            signVal = Mathf.Sign(playerInput.y);
            rb.AddTorque(forceAmount * -signVal);
        }

        if (playerInput.x != 0 && Mathf.Abs(rb.angularVelocity) < 0.25f)
        {
            additivePos.x = Vector3.right.x * Mathf.Sign(playerInput.x);
            transform.position += (additivePos * horizontalSpeed * Time.deltaTime);
        }

        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVel, maxAngularVel);

        int temp = pieces.Count - 1;
        if (rb.angularVelocity < 0) temp = 0;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (rb.angularVelocity < maxImpactVel)
        {
            if (col.otherCollider.GetType() == typeof(CircleCollider2D))
            {
                CircleCollider2D circleCol = (CircleCollider2D)col.otherCollider;
                if (circleCol.radius == 0.4f)
                {
                    Transform hit = col.otherCollider.transform;
                    Transform breakingPoint;
                    hitIndex = pieces.IndexOf(hit);

                    if (hitIndex != 0)
                    {
                        breakingPoint = pieces[hitIndex - 1];
                        hit.position = (Vector2)pieces[hitIndex - 2].position - anchorOffset;
                    } else
                    {
                        breakingPoint = pieces[hitIndex + 1];
                        hit.position = (Vector2)pieces[hitIndex + 2].position + anchorOffset;
                    }

                    pieces.Remove(breakingPoint);
                    Destroy(breakingPoint.gameObject);
                }
            }
        }
    }
}
