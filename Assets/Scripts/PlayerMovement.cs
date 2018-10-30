using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance;

    public Vector3 anchorOffset;
    public float horizontalSpeed;
    public float moveSpeed;
    public float forceAmount;
    public int maxAngularVel;
    public float maxImpactVel;

    float horizontalCooldownTimer = 0;
    float signVal;
    float breakDelayTimer;
    float forceChangeIncrement;

    int hitIndex;

    Vector2 playerInput;
    Vector3 additivePos;
    Rigidbody2D rb;
    public List<Transform> pieces;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        playerInput = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
        pieces = GetComponentsInChildren<Transform>().ToList();
        pieces.Remove(transform);

        signVal = 0;

        pieces[0].localPosition = pieces[1].localPosition - anchorOffset;
        pieces[pieces.Count - 1].localPosition = pieces[pieces.Count - 2].localPosition + anchorOffset;

        forceChangeIncrement = (forceAmount / pieces.Count) * 0.7f;
	}
	
	void Update () {
        playerInput.x = Input.GetAxis("Horizontal");
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
            additivePos.x = Vector3.right.x * Mathf.Sign(playerInput.x) * (moveSpeed * Time.deltaTime);
            transform.position += (additivePos * horizontalSpeed * Time.deltaTime);
        }

        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVel, maxAngularVel);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (pieces.Count == 1) return;

        if (Mathf.Abs(col.relativeVelocity.x) > maxImpactVel && Mathf.Abs(col.relativeVelocity.y) > maxImpactVel && breakDelayTimer < Time.timeSinceLevelLoad)
        {
            if (pieces.Count == 3)
            {
                Destroy(pieces[0].gameObject);
                Destroy(pieces[2].gameObject);

                pieces.Remove(pieces[0]);
                pieces.Remove(pieces[1]);

                return;
            }

            /// First, check if ends
            /// Seconds, handle between hits
            /// If it's not circle collider, has to be between
            Transform hit = col.otherCollider.transform;
            Transform breakingPoint;
            hitIndex = pieces.IndexOf(hit);

            if (col.otherCollider.GetType() == typeof(CircleCollider2D))
            {
                print(col.otherCollider.gameObject.name + "  VEL: " + col.relativeVelocity);
                CircleCollider2D circleCol = (CircleCollider2D)col.otherCollider;
                    if (hitIndex != 0)
                    {
                        breakingPoint = pieces[hitIndex - 1];
                        hit.localPosition = pieces[hitIndex - 2].localPosition - anchorOffset;
                    } else
                    {
                        breakingPoint = pieces[1];
                        hit.localPosition = pieces[2].localPosition - anchorOffset;
                    }

                    pieces.Remove(breakingPoint);
                    Destroy(breakingPoint.gameObject);

                    forceAmount -= forceChangeIncrement;
                    breakDelayTimer = Time.timeSinceLevelLoad + 2;
            }
            else
            {
                List<Transform> toBeRemoved = new List<Transform>();

                pieces[0].gameObject.SetActive(false);
                pieces[pieces.Count - 1].gameObject.SetActive(false);
                if (hitIndex < pieces.Count / 2)
                {
                    for(int i = 0; i <= hitIndex; i++)
                    {
                        if (pieces[i].tag == "Ball")
                        {
                            Vector3 currentPos = pieces[hitIndex + 1].localPosition;
                            print(pieces[hitIndex + 1]);
                            pieces[i].localPosition = new Vector3(currentPos.x - anchorOffset.x, currentPos.y, 0);
                            continue;
                        }
                        toBeRemoved.Add(pieces[i]);
                    }
                } else
                {
                    for(int i = hitIndex; i < pieces.Count; i++)
                    {
                        if (pieces[i].tag == "Ball")
                        {
                            Vector3 currentPos = pieces[hitIndex - 1].localPosition;
                            pieces[i].localPosition = new Vector3(currentPos.x + anchorOffset.x, currentPos.y, 0);
                            continue;
                        }

                        toBeRemoved.Add(pieces[i]);
                    } 
                }

                pieces[0].gameObject.SetActive(true);
                pieces[pieces.Count - 1].gameObject.SetActive(true);

                BreakOff(toBeRemoved);

                breakDelayTimer = Time.timeSinceLevelLoad + 3;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "Floor")
        {
            LevelManager.Instance.ChangeRoom(coll.transform);
        }
    }

    void BreakOff(List<Transform> orphans)
    {
        int newSize = pieces.Count - orphans.Count;

        if(newSize == 3)
        {
            Transform anchor1 = pieces[0];
            Transform anchor2 = pieces[pieces.Count - 1];
            orphans.Add(anchor1);
            orphans.Add(anchor2);

            Destroy(anchor1.gameObject);
            Destroy(anchor2.gameObject);
        }
        foreach(Transform orphan in orphans)
        {
            orphan.parent = null;
            orphan.gameObject.AddComponent<Rigidbody2D>();
            pieces.Remove(orphan);
        }

        forceAmount -= (forceChangeIncrement * orphans.Count);
    }
}
