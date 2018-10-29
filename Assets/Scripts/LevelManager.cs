using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public Transform currentFloor;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
		
	}
	
	void Update () {
		
	}

    public void ChangeRoom(Transform nextRoom)
    {
        if (nextRoom.name != currentFloor.name)
        {
            CameraController.Instance.MoveToRoom(nextRoom);
            currentFloor = nextRoom;
        }
    }
}
