using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    public Transform currentFloor;
    public GameObject introText;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        introText.SetActive(true);
	}
	
	void Update () {
		if(Input.GetKey(KeyCode.Space))
        {
            introText.SetActive(false);
        }
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
