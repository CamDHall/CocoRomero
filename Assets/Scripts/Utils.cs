using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    public static float CalculateGravity(float mass, float gravity)
    {
        return Mathf.Sqrt(2 * mass * gravity);
    }
}
