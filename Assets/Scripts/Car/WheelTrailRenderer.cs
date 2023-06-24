using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRenderer : MonoBehaviour
{
    public bool isOverpassEmiting = false;
    TopDownCarController carController;
    CarLayerHandler carLayerHandler;
    TrailRenderer trail;
    private void Awake() {
        carController = GetComponentInParent<TopDownCarController>();

        trail = GetComponent<TrailRenderer>();

        carLayerHandler = GetComponentInParent<CarLayerHandler>();  
        trail.emitting = false;
        trail.time = 2f;
    }
    

    // Update is called once per frame
    void Update()
    {
        trail.emitting = false;
        
        //If car is screeching so emit the trail
        if(carController.IsTireScreeching(out float lateralVelocity, out bool isBraking)){

            if (carLayerHandler.IsDrivingOverpass())
            {

            }
            trail.emitting = true;
        }
        else{
            trail.emitting = false;
        }
    }
}
