using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [Header("Speed that we set to the ai car when reach this waypoint")]
    public float maxSpeed = 0;
    
    [Header("Next waypoint distance")]
    public float minDistanceToNextWaypoint = 5;
    public WaypointNode[] nextWaypointNode;

}
