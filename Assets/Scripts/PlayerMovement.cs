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

        if (col.transform.tag == "Blade") {
            BreakOff(new List<Transform> { col.otherCollider.transform });
            return;
        }

        if (Mathf.Abs(col.relativeVelocity.x) > maxImpactVel && Mathf.Abs(col.relativeVelocity.y) > maxImpactVel && breakDelayTimer < Time.timeSinceLevelLoad)
        {
            if (pieces.Count == 3)
            {
                Destroy(pieces[0].gameObject);
                Destroy(pieces[2].gameObject);

                pieces.Remove(pieces[0]);
                pieces.Remove(pieces[1]);

                breakDelayTimer = Time.timeSinceLevelLoad + 2;
                return;
            }

            /// First, check if ends
            /// Seconds, handle between hits
            /// If it's not circle collider, has to be between
            Transform hit = col.otherCollider.transform;
            hitIndex = pieces.IndexOf(hit);

            bool end1Less = pieces[0].position.y < col.transform.position.y;
            bool end2Less = pieces[pieces.Count - 1].position.y < col.transform.position.y;

            if ((end1Less && end2Less) || (!end1Less && !end2Less)) {
                BreakOff(new List<Transform>() { hit });
                breakDelayTimer = Time.timeSinceLevelLoad + 2;
                print("HERE");
                return;
            }

            if (col.otherCollider.GetType() == typeof(CircleCollider2D))
            {
                List<Transform> toBeRemoved = new List<Transform>(pieces);

                if(hitIndex == 0)
                {
                    toBeRemoved.RemoveAt(hitIndex + 1);
                } else
                {
                    toBeRemoved.RemoveAt(hitIndex -1);
                }

                BreakOff(toBeRemoved);

                forceAmount -= forceChangeIncrement;
                breakDelayTimer = Time.timeSinceLevelLoad + 2;
            }
            else
            {
                if (transform.rotation.z > 0)
                {
                    List<Transform> broken = pieces.GetRange(1, hitIndex - 1);
                    BreakOff(broken);
                }
                else
                {
                    int len = pieces.Count - 1 - hitIndex;
                    if (len == hitIndex)
                    {
                        List<Transform> broken = pieces.GetRange(hitIndex + 1, len);
                        BreakOff(broken);
                    } else
                    {
                        List<Transform> broken = pieces.GetRange(hitIndex + 1, (pieces.Count - 1) - hitIndex);
                        BreakOff(broken);
                    }
                }
            }

            breakDelayTimer = Time.timeSinceLevelLoad + 2;
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
        foreach (Transform orphan in orphans)
        {
            //print(orphan.name);
            if (orphan.tag == "Ball")
            {
                pieces.Remove(orphan);
                Destroy(orphan.gameObject);
                continue;
            }

            orphan.parent = null;
            orphan.gameObject.AddComponent<Rigidbody2D>();
            pieces.Remove(orphan);
        }

        forceAmount -= (forceChangeIncrement * orphans.Count);

        // Anchor fixes
        if (pieces.Count == 3)
        {
            Destroy(pieces[0].gameObject);
            Destroy(pieces[2].gameObject);

            pieces.Remove(pieces[0]);
            pieces.Remove(pieces[1]);
        } else if(pieces.Count == 2)
        {
            if(pieces[0].tag == "Ball")
            {
                Destroy(pieces[0].gameObject);
                pieces.RemoveAt(0);
            } else
            {
                Destroy(pieces[1].gameObject);
                pieces.RemoveAt(1);
            }
        }
        
        if(pieces.Count > 2)
        {
            pieces[0].localPosition = pieces[1].localPosition - anchorOffset;
            pieces[pieces.Count - 1].localPosition = pieces[pieces.Count - 2].localPosition + anchorOffset;
        }
    }
}
