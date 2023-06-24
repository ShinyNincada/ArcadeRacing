using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    //Other Components
    LeaderboardUIHandler leaderboardUIHandler;
    HighscoreBoard highScoreUI;
    public List<CarLapCounter> carLaps = new List<CarLapCounter>();
    // Start is called before the first frame update
    private void Start() {
       
           //Find all car lap in the scene 
        CarLapCounter[] carLapArray = FindObjectsOfType<CarLapCounter>();

        //Change carlap array into list
        carLaps = carLapArray.ToList<CarLapCounter>();

        //Hook up the passed checkpoint event
        foreach(CarLapCounter carLap in carLaps) {
            // UnityAction += để nối với function được truyền vào -> gọi function khi action được invoke
            carLap.onPassCheckpoint += OnPassCheckPoint;
        }

        leaderboardUIHandler = FindObjectOfType<LeaderboardUIHandler>();
        highScoreUI = FindObjectOfType<HighscoreBoard>();
        if(leaderboardUIHandler){
            leaderboardUIHandler.LeaderboardInit(carLapArray);
            //Ask the leaderboard handler to update the list
            leaderboardUIHandler.UpdateLeaderboard(carLaps);
        }

        // highScoreUI?.LeaderboardInit();
       
    }

    void OnPassCheckPoint(CarLapCounter carLapCounter)
    {
        //Update lại thứ tự xe theo số checkpoint đã qua + thời gian hoàn thành sớm nhất
        // print($"Event: Car {carLapCounter.gameObject.name} passed checkpoint");
        carLaps = carLaps.OrderByDescending(s => s.GetCheckpointPassedCount()).ThenBy(s => s.GetTimePassedLastCheckpoint()).ToList();
        
        //Get the car position
        int carPosition = carLaps.IndexOf(carLapCounter) + 1; //Vì start index = 0 

        //Cập nhật vị trí mới cho LapCounter
        carLapCounter.SetCarPosition(carPosition);

        //
        leaderboardUIHandler?.UpdateLeaderboard(carLaps);

        if(carLapCounter.IsRaceCompleted()){
            //Set players last position
            int playerNumber = carLapCounter.GetComponent<CarInputHandler>().playerNumber;
            GameManager.instance.SetDriverLastRacePosition(playerNumber, carPosition);

            //Add point to champion
            int championshipPointAwarded = GameManager.instance.GetDriverList().Count - carPosition;
            GameManager.instance.AddPointsToChampion(playerNumber, championshipPointAwarded);
            GameManager.instance.SetNewMapLeaderboard(carLapCounter.gameObject.name, carLapCounter.timePassedLastCheckpoint);
            
            if(carLapCounter.gameObject.CompareTag("Player")){
                highScoreUI?.LeaderboardInit();
            }
        }
    }
}
