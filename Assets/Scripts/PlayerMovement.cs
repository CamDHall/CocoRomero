using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float horizontalSpeed;
    public float _horizontalCooldownAmount;
    public float forceAmount;

    Vector2 playerInput;
    Vector2 additivePos;
    Vector2 offSetVect = new Vector2(-0.5f, -0.5f);
    Rigidbody2D rb;
    float horizontalCooldownTimer = 0;



    void Start () {
        playerInput = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
        Debug.Log(horizontalCooldownTimer);
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
        playerInput.y = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
    }

    private void FixedUpdate()
    {
        additivePos = new Vector2(playerInput.x, 0);

        transform.Translate(additivePos * horizontalSpeed * Time.deltaTime);
        rb.AddForceAtPosition(new Vector2(0, playerInput.y * forceAmount), offSetVect, ForceMode2D.Force);
        //rb.AddForce(new Vector2(0, playerInput.y * forceAmount));
    }
}
