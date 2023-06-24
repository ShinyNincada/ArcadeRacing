using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class LeaderboardUIHandler : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;

    public SetLeaderboardItemInfo[] leaderboardItems;

    public void UpdateLeaderboard(List<CarLapCounter> lapCounters){
        //Create the leaderboard items
        for(int i = 0; i < lapCounters.Count; i++){
            leaderboardItems[i].SetNameText(lapCounters[i].gameObject.name);
        }
    }

    public void LeaderboardInit( CarLapCounter[] carLapCounterArray){
        VerticalLayoutGroup leaderboardLayout = GetComponentInChildren<VerticalLayoutGroup>(); 
   
        //Allocate the array
        leaderboardItems = new SetLeaderboardItemInfo[carLapCounterArray.Length];
        for(int i = 0; i < carLapCounterArray.Length; i++){
            
            //Instantiate an leaderboard items inside the leaderboard
            GameObject leaderboardGameObject = Instantiate(leaderboardItemPrefab, leaderboardLayout.transform);
            leaderboardItems[i] = leaderboardGameObject.GetComponent<SetLeaderboardItemInfo>();

            leaderboardItems[i].SetPositionText($"{i + 1}.");

          
        }

       
    }
}
