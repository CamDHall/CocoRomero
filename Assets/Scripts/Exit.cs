using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    public Transform roomFloor;
    public Transform nextFloor;

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.transform.tag == "Player")
        {
            if (PlayerMovement.Instance.isOnVerticalBelt)
            {
                PlayerMovement.Instance.ResetFromBelt(transform.localPosition, nextFloor);
                CameraController.Instance.MoveToRoom(nextFloor);
            }
            else
            {
                if(PlayerMovement.Instance.currentFloor == nextFloor)
                {
                    CameraController.Instance.MoveToRoom(roomFloor);
                } else
                {
                    CameraController.Instance.MoveToRoom(nextFloor);
                }
            }
        }
    }
}
