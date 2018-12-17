using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void MoveToRoom(Transform nextRoomPos)
    {
        float heightDiff = nextRoomPos.position.y - PlayerMovement.Instance.currentFloor.position.y;

        float yValue = transform.position.y + heightDiff;
        Vector3 newPos = new Vector3(nextRoomPos.position.x, yValue, -10);
        transform.position = newPos;

        PlayerMovement.Instance.currentFloor = nextRoomPos;
    }
}
