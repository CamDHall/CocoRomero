﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance;

    public Transform currentFloor;
    public float horizontalSpeed;
    public float moveSpeed;
    public float forceAmount;
    public int maxAngularVel;
    public float maxImpactVel;

    [HideInInspector] public bool isOnVerticalBelt;

    float signVal;
    float breakDelayTimer;
    float forceChangeIncrement;
    float prevPos;

    Vector2 additivePos;
    Vector2 playerInput;
    bool onRamp;

    Rigidbody2D rb;
    public List<Transform> pieces;
    List<ParticleCollisionEvent> hitEvents;

    private void Awake()
    {
        Instance = this;
        hitEvents = new List<ParticleCollisionEvent>();
    }

    void Start () {
        playerInput = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
        pieces = GetComponentsInChildren<Transform>().ToList();
        pieces.Remove(transform);

        signVal = 0;
        forceChangeIncrement = (forceAmount / pieces.Count) * 0.85f;
        prevPos = 0;

        CameraController.Instance.MoveToRoom(currentFloor);
	}
	
	void Update () {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        if(isOnVerticalBelt)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, 
                new Vector3(0, transform.localPosition.y, 0), Time.deltaTime * 5);

            transform.localRotation = Quaternion.Lerp(transform.localRotation, 
                Quaternion.Euler(new Vector3(0, 0, 90)), Time.deltaTime * 5);
        }
    }

    private void FixedUpdate()
    {
        additivePos = Vector2.zero;

        if (playerInput.y != 0 && !isOnVerticalBelt)
        {
            signVal = Mathf.Sign(playerInput.y);
            float currentForceAmount = Time.deltaTime* forceAmount *-signVal; 

            if(onRamp)
            {
                currentForceAmount++;
            }

            rb.AddTorque(currentForceAmount, ForceMode2D.Impulse);
        }

        if (isOnVerticalBelt)
        {
            additivePos.y = Vector3.up.y * Mathf.Sign(playerInput.x) * (moveSpeed * Time.deltaTime);
        
            transform.position += ((Vector3)additivePos * horizontalSpeed * Time.deltaTime);
        } else if (playerInput.x != 0 && Mathf.Abs(rb.angularVelocity) < 0.5f)
        {
            if(!isOnVerticalBelt)
            {
                additivePos.x = Vector3.right.x * Mathf.Sign(playerInput.x) * (moveSpeed * Time.deltaTime);
            }

            transform.position += ((Vector3)additivePos * horizontalSpeed * Time.deltaTime);
        }

        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVel, maxAngularVel);
        prevPos = rb.position.x;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Platform")
        {
            transform.parent = col.transform.parent;
        }
        else
        {
            transform.parent = null;
        }

        if (col.transform.tag == "Ramp")
        {
            onRamp = true;
        }
        else
        {
            onRamp = false;
        }

        if (pieces.Count == 1) return;

        if (col.transform.tag == "Blade") {
            BreakOff(new List<Transform> { col.otherCollider.transform });
            return;
        }

        if(col.transform.tag == "Door")
        {
            List<Transform> toBeRemoved = new List<Transform>();
            if(prevPos < 0)
            {
                print(col.otherCollider.name);
                if (col.otherCollider.transform.position.x > col.transform.position.x) return;

                foreach(Transform piece in pieces)
                {
                    if(piece.transform.position.x > col.otherCollider.transform.position.x)
                    {
                        toBeRemoved.Add(piece);
                    }
                }
            } else
            {
                if (col.otherCollider.transform.position.x > col.transform.position.x) return;
                foreach (Transform piece in pieces)
                {
                    if (piece.transform.position.x < col.otherCollider.transform.position.x)
                    {
                        toBeRemoved.Add(piece);
                    }
                }
            }

            BreakOff(toBeRemoved);
            return;
        }

        float relativeX = Mathf.Abs(col.relativeVelocity.x);
        float relativeY = Mathf.Abs(col.relativeVelocity.y);

        if ((relativeX > maxImpactVel || (relativeY > maxImpactVel && relativeX > 1.3f))
            && breakDelayTimer < Time.timeSinceLevelLoad)
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

            Transform hit = col.otherCollider.transform;

            bool end1Less = pieces[0].position.y < col.transform.position.y;
            bool end2Less = pieces[pieces.Count - 1].position.y < col.transform.position.y;

            if ((end1Less && end2Less) || (!end1Less && !end2Less)) {
                BreakOff(new List<Transform>() { hit });
                breakDelayTimer = Time.timeSinceLevelLoad + 2;
                return;
            }

            List<Transform> broken = new List<Transform>();

            foreach(Transform piece in pieces)
            {
                if(piece.position.y < hit.transform.position.y)
                {
                    broken.Add(piece);
                }
            }

            BreakOff(broken);

            breakDelayTimer = Time.timeSinceLevelLoad + 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Belt")
        {
            transform.parent = col.transform.parent;
            ConveyorBelt belt = col.transform.parent.GetComponent<ConveyorBelt>();

            if (belt.beltType == ObstacleType.Vertical)
            {
                isOnVerticalBelt = true;
                rb.gravityScale = 0;
            }
        }
    }

    public void ResetFromBelt(Vector3 startingPoint, Transform newFloor)
    {
        transform.parent = newFloor;
        transform.localPosition = new Vector3(startingPoint.x, 0.5f, 0);
        transform.localRotation = Quaternion.identity;
        rb.gravityScale = 1;

        isOnVerticalBelt = false;
    }

    void BreakOff(List<Transform> orphans)
    {
        foreach (Transform orphan in orphans)
        {
            orphan.parent = null;
            orphan.gameObject.AddComponent<Rigidbody2D>();
            pieces.Remove(orphan);
        }

        forceAmount -= (forceChangeIncrement * orphans.Count);
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = other.GetComponent<ParticleSystem>();
        int hitNum = ps.GetCollisionEvents(gameObject, hitEvents);

        for (int i = 0; i < hitNum; i++)
        {
            Vector3 pos = hitEvents[i].intersection;

            Collider2D[] hits = Physics2D.OverlapBoxAll(pos, Vector2.one, 0);

            List<Transform> toBeMelted = new List<Transform>();

            foreach (Collider2D hit in hits) toBeMelted.Add(hit.transform);

            BreakOff(toBeMelted);
        }

    }
}
