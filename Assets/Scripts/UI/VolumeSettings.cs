using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SfxSlider;

    private void Awake() {
        
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SfxSlider.onValueChanged.AddListener(SetSfxVolume);

    }

    private void OnEnable() {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY);
        SfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY);
    }
   
    void SetMusicVolume(float value){
        AudioManager.instance.SetMusicVolume(value);
    }

    void SetSfxVolume(float value){
        AudioManager.instance.SetSfxVolume(value);
    }

}
