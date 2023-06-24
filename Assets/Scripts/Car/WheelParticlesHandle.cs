using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelParticlesHandle : MonoBehaviour
{
    //Local variables
    float particleEmmissionRate = 0;

    //Component
    TopDownCarController carController;
    ParticleSystem smokeParticleSystem;
    ParticleSystem.EmissionModule emissionModule;

    private void Awake() {
        //Get the car controller
        carController = GetComponentInParent<TopDownCarController>();
        
        //Get the particles system
        smokeParticleSystem = GetComponent<ParticleSystem>();

        //get particle system emission
        emissionModule = smokeParticleSystem.emission;

        //set it to zero
        emissionModule.rateOverTime = 0;
    }
    

    // Update is called once per frame
    void Update()
    {
        //Reduce the particle over time
        particleEmmissionRate = Mathf.Lerp(particleEmmissionRate, 0, Time.deltaTime * 5);
        emissionModule.rateOverTime = particleEmmissionRate;

        if(carController.IsTireScreeching(out float lateralVelocity, out bool isBraking)){
            //If the car tires are screeching then we'll emitt smoke. If the player is braking then emitt a lot of smoke.
            if(isBraking){
                particleEmmissionRate = 20;
            }
            //If the player is drifting we'll emitt smoke based on how much the player is drifting.
            else particleEmmissionRate = Mathf.Abs(lateralVelocity) * 2;
        }
    }

    //Image by <a href="https://www.freepik.com/free-vector/cartoon-smoke-element-animation-frames_13763535.htm#query=smoke%20puff&position=17&from_view=keyword&track=ais">Freepik</a>
}
