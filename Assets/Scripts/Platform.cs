using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : Obstacles {

    public ObstacleType obstacleType;

    delegate void Move();

    Move platformMove;

	new void Start () {
        base.Start();

		if(obstacleType == ObstacleType.Horiztonal)
        {
            platformMove = MoveHoritonzal;
        } else if(obstacleType == ObstacleType.Vertical)
        {
            platformMove = MoveVertical;
        }
	}
	
	void FixedUpdate () {
        platformMove();
	}
}
