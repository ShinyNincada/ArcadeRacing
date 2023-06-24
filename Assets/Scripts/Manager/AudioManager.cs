using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioMixer mixer;

    public const string MUSIC_KEY = "MusicVolume";
    public const string SFX_KEY = "SFXVolume";
    public float MusicVolume;
    public float SfxVolume;
    void Start(){
        if(instance == null){
            instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }

        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_KEY));
        SetSfxVolume(PlayerPrefs.GetFloat(SFX_KEY));
    }

    void LoadVolume(){
        Debug.Log($"Mussic player: {PlayerPrefs.GetFloat(MUSIC_KEY)}");
        Debug.Log(PlayerPrefs.GetFloat(SFX_KEY));
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY);
        SfxVolume = PlayerPrefs.GetFloat(SFX_KEY);

        SetMusicVolume(MusicVolume);
        SetSfxVolume(SfxVolume);
    }

    public void SetMusicVolume(float value){
        if (value > 0){
            mixer.SetFloat(MUSIC_KEY, Mathf.Log10(value) * 20);
        }
        else {
            mixer.SetFloat(MUSIC_KEY, -80); // Set a very low volume when value is 0
        }
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    public void SetSfxVolume(float value){
        if (value > 0){
            mixer.SetFloat(SFX_KEY, Mathf.Log10(value) * 20);
        }
        else {
            mixer.SetFloat(SFX_KEY, -80); // Set a very low volume when value is 0
        }
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }
}
