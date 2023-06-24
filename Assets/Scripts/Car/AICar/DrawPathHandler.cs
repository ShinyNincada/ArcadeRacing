using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPathHandler : MonoBehaviour
{
    public Transform RootObjectTransform;
    WaypointNode[] nodes;
   
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;

        if(RootObjectTransform == null){
            return;
        }

        //Get all waypoint
        nodes = GetComponentsInChildren<WaypointNode>();

        //Iterator all the node
        foreach(WaypointNode node in nodes){
            //Iterator all the next node possible and draw gizmos
            foreach(WaypointNode next in node.nextWaypointNode){
                if(next != null){
                    Gizmos.DrawLine(node.transform.position, next.transform.position);
                }
            }
        }
    }
}
