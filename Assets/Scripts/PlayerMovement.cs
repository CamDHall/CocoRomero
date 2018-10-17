using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float horizontalSpeed;
    public float _horizontalCooldownAmount;
    public float forceAmount;

    float horizontalCooldownTimer = 0;
    float signVal;

    Vector2 playerInput;
    Vector3 additivePos;
    Vector2 offset = new Vector2(-0.5f, -0.5f);
    Rigidbody2D rb;


    void Start () {
        playerInput = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate()
    {
        additivePos = Vector3.zero;

        if(playerInput.x != 0)
        {
            additivePos.x = Vector3.right.x * Mathf.Sign(playerInput.x);
        }

        transform.position += (additivePos * horizontalSpeed * Time.deltaTime);
        if (playerInput.y != 0)
        {
            signVal = Mathf.Sign(playerInput.y);
            offset = new Vector2(transform.localPosition.x + (signVal * -0.5f), transform.localPosition.y - 0.5f);
            rb.AddTorque(forceAmount * -signVal);
        }
        //rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, 0, 50);

        Debug.Log(rb.angularVelocity);
    }
}
