using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLapCounter : MonoBehaviour
{
    //Checkpoint và thời gian vượt gần đây nhất
    public int lastPassedCheckpoint = 0;
    public float timePassedLastCheckpoint = 0;

    //Tổng số cp đã vượt
    int checkPointPassedCount = 0;

    //Số vòng để hoàn thành và số vòng đã đi
    const int lapToComplete = 3;
    public int lapCompleted = 0;
    public Vector2 lastCPPosition;
    //Biến kiểm tra đã đua xong hay chưa
    bool isRaceCompleted = false;
    int carPosition = 0;

    // bool isHideRoutineRunning = false;
    // float hideUIDelayTime;
    public RaceTimeUIHandler timeUIHandler;

    //Other component
    LapCounterUIHandler lapCounterUIHandler;

    //Events
    public event Action<CarLapCounter>  onPassCheckpoint;

    private void Start() {
        lastCPPosition = transform.position;
        if(CompareTag("Player")){
            lapCounterUIHandler = FindObjectOfType<LapCounterUIHandler>();
            lapCounterUIHandler.SetLapText($"LAP {lapCompleted + 1}/{lapToComplete}");
        }

        timeUIHandler = FindObjectOfType<RaceTimeUIHandler>();
    }

    public void SetCarPosition(int position)
    {
        carPosition = position;
    }

    public int GetCheckpointPassedCount()
    {
        return checkPointPassedCount;
    }

    public float GetTimePassedLastCheckpoint()
    {
        return timePassedLastCheckpoint;    
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("CheckPoint"))
        {
            //Once a car has complete the race, doesn't need to check other lap or checkpoint
            if(isRaceCompleted)
            {
                return;
            }
            Checkpoint checkpoint = col.GetComponent<Checkpoint>(); 

            //Make sure player car is passing checkpoints correctly 1 -> 2 -> 3 -> ...
            if(lastPassedCheckpoint + 1 == checkpoint.checkPointNumber)
            {
                lastPassedCheckpoint = checkpoint.checkPointNumber;
                checkPointPassedCount++;

                //Store the time at checkpoint
                timePassedLastCheckpoint = timeUIHandler.GetRaceTime();
                lastCPPosition = col.transform.position;

                if (checkpoint.isFinishLine)
                {
                    lastPassedCheckpoint = 0;
                    lapCompleted++;

                    if(lapCompleted == lapToComplete)
                    {
                        isRaceCompleted = true;
                    }

                    if(!isRaceCompleted && lapCounterUIHandler != null){
                        lapCounterUIHandler.SetLapText($"LAP {lapCompleted + 1}/{lapToComplete}");
                    }
                }

                //Invoke the passed checkpoint event
                onPassCheckpoint?.Invoke(this);

                //SHow the 1st car on the leaderboard after the race is completed
                if(isRaceCompleted){
                    if(CompareTag("Player")){
                        GameManager.instance.OnRaceCompleted();
                    }
                    // ShowPositionCO();
                }
                else if(checkpoint.isFinishLine){
                    // ShowCarPosition();
                }
            }
        }
    }

    public bool IsRaceCompleted(){
        return isRaceCompleted;
    }
    // async void ShowPositionCO(){
         
    // }

    // async void ShowCarPosition(){
        
    // }
}
