using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CarSfxHandler : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer mixer;

    
    [Header("Audio Sources")]
    public AudioSource carEnginegAudio;
    public AudioSource tireScreeachingAudio;
    public AudioSource carHitAudio;
    public AudioSource carJumpAudio;
    public AudioSource carLandingAudio;

    [Header("Item SFX")]
    public AudioSource snowballSFX;
    public AudioSource getItemSFX;

    //local variables
    float  desiredEnginePitch = 0.5f;
    float  tireScreechPitch = 0.5f;

    //Components
    TopDownCarController carController;
    
    // Start is called before the first frame update
    void Awake()
    {
        carController = GetComponentInParent<TopDownCarController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEngineSFX();
        UpdateTireSchreechingSFX();
    }

    void UpdateEngineSFX(){
        //Handle engine SFX
        float velocityMag = carController.GetVelocityMagnitude();

        //Increase the engines volume as the car move faster
        float desiredEngineVolume = velocityMag * 0.05f;

        //But keep a minimum level so it plays even when the car is idle
        desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.2f, 1.0f);

        carEnginegAudio.volume = Mathf.Lerp(carEnginegAudio.volume, desiredEngineVolume, Time.deltaTime * 10);

        //To add more variation to the engine sound, we also change the pitch
        desiredEnginePitch = velocityMag * 0.2f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.5f, 2f);
        carEnginegAudio.pitch = Mathf.Lerp(carEnginegAudio.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);
    }

    void UpdateTireSchreechingSFX(){
        //Handle the tire screeching SFX
        if(carController.IsTireScreeching(out float lateralVelocity, out bool isBraking)){
            //If the car is screeching, we want the sound louder and also change the pitch
            if(isBraking){
                tireScreeachingAudio.volume = Mathf.Lerp(tireScreeachingAudio.volume, 1.0f, Time.deltaTime * 10);
                tireScreechPitch = Mathf.Lerp(tireScreechPitch, 0.5f, Time.deltaTime * 10);
            }
            else{
                //If we are not braking we still want to play screech sound if player is drifting
                tireScreeachingAudio.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
        }
         //Fade out the tire screech SFX if we are not screeching. 
        else tireScreeachingAudio.volume = Mathf.Lerp(tireScreeachingAudio.volume, 0, Time.deltaTime * 10);
    }

    public void PlayJumpSFX(){
        carJumpAudio.Play();
    }
    
    public void PlayLandingSFX(){
        carLandingAudio.Play();
    }

    public void PlaySnowBallSFX(){
        snowballSFX.Play();
    }

    public void PlayGetItemSFX(){
        getItemSFX.Play();
    }
    private void OnCollisionEnter2D(Collision2D col) {
        //Get the relative velocity of the collision
        float relativeVelocity = col.relativeVelocity.magnitude;

        float volume = relativeVelocity * 0.1f;

        carHitAudio.volume = volume;
        carHitAudio.pitch = Random.Range(0.95f, 1.05f);        

        if(!carHitAudio.isPlaying)
            carHitAudio.Play();
    }
}
