using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour {

    public float horizontalSpeed;
    public float _horizontalCooldownAmount;
    public float forceAmount;
    public int maxAngularVel;
    public int maxImpactVel;

    float horizontalCooldownTimer = 0;
    float signVal;
    float breakDelayTimer;
    int anchorIndex;

    Vector2 playerInput;
    Vector3 additivePos;
    Rigidbody2D rb;
    Transform anchorPiece;
    List<Transform> pieces;


    void Start () {
        playerInput = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
        pieces = GetComponentsInChildren<Transform>().ToList();
        pieces.Remove(transform);
        signVal = 0;
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
        if(playerInput.y < 0)
        {
            anchorIndex = 0;
        } else
        {
            anchorIndex = pieces.Count - 1;
        }

        anchorPiece = pieces[anchorIndex];
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
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(breakDelayTimer < Time.timeSinceLevelLoad && Mathf.Abs(rb.angularVelocity) > maxImpactVel)
        {
            //ContactPoint2D[] hits = col.contacts;
            //foreach(ContactPoint2D contact in hits)
            //{
            //    if(contact.otherCollider.transform.name != anchorPiece.name)
            //    {
            //        print("break this one: " + contact.otherCollider.transform.name);
            //    }
            //}
            Transform hit = col.GetContact(0).otherCollider.transform;
            int index = pieces.IndexOf(hit);
            if(index > 0 && pieces[index -1].position.y > hit.transform.position.y ||
                index < pieces.Count - 1 && index > 0 && pieces[index + 1].position.y > hit.transform.position.y)
            {
                return;
            }

            if(index == 0 && pieces[index + 1].position.y < hit.position.y)
            {
                return;
            }

            Debug.Log(hit.name);

            pieces.Remove(hit);
            Destroy(hit.gameObject);
            breakDelayTimer = Time.timeSinceLevelLoad + 3;
        }
    }
}
