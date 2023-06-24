using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SfxUIHandler : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer mixer;

    
    [Header("Audio Sources")]
    public AudioSource buttonClickAudio;
    public AudioSource buttonRolloverAudio;
    
    public void PlayOnClickSFX(){
        buttonClickAudio.Play();
    }

    public void PlayOnRolloverSFX(){
        buttonRolloverAudio.Play();
    }
}
