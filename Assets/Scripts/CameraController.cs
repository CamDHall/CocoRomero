using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }


    void Update () {

	}

    public void MoveToRoom(Transform nextRoomPos)
    {
        transform.position = new Vector3(nextRoomPos.position.x, transform.position.y + nextRoomPos.localPosition.y, -10);
    }
}
