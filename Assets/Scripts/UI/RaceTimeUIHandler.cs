using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
public class RaceTimeUIHandler : MonoBehaviour
{
    TMP_Text timeText;
    
    float lastRaceTimeUpdate = 0;
    float currentRaceTime;

    void Awake(){
        timeText = GetComponent<TMP_Text>();
    }

   void Start(){
        UpdateTimeCO();
   }

   async void UpdateTimeCO(){
        while(true){
            float raceTime = GameManager.instance.getRaceTime();

            if(lastRaceTimeUpdate != raceTime){
                int raceTimeMinutes = (int)Mathf.Floor(raceTime/60);
                int raceTimeSeconds = (int)Mathf.Floor(raceTime%60);

                timeText.text = $"{raceTimeMinutes.ToString("00")}:{raceTimeSeconds.ToString("00")}";
                currentRaceTime = raceTimeMinutes + raceTimeSeconds;
                lastRaceTimeUpdate = raceTime;
            }

            await Task.Delay(100);
        }
   }

   public float GetRaceTime(){
        return currentRaceTime;
   }
}
