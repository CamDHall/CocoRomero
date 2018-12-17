using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance;
    public Transform currentFloor;

    private void Awake()
    {
        Instance = this;
        MoveToRoom(currentFloor);
    }

    public void MoveToRoom(Transform nextRoomPos)
    {
        float heightDiff = nextRoomPos.position.y - currentFloor.position.y;

        float yValue = transform.position.y + heightDiff;
        Vector3 newPos = new Vector3(nextRoomPos.position.x, yValue, -10);
        transform.position = newPos;

        currentFloor = nextRoomPos;
    }
}
