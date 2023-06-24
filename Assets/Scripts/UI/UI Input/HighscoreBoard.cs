using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HighscoreBoard : MonoBehaviour
{
    public GameObject highscoreItemPrefab;
    public SetLeaderboardItemInfo[] highscoreItems;
    SaveManager.MapLeaderboard currentMapLeaderboard;
    public void UpdateLeaderboard(){

        //Setup info for leaderboard item
        for(int i = 0; i < currentMapLeaderboard.leaderboard.Count; i++){
            highscoreItems[i].SetNameText(currentMapLeaderboard.leaderboard[i].name);
            highscoreItems[i].SetTimeText(currentMapLeaderboard.leaderboard[i].time);
        }
    }

    public void LeaderboardInit(){
        VerticalLayoutGroup leaderboardLayout = GetComponentInChildren<VerticalLayoutGroup>(); 
        currentMapLeaderboard = GameManager.instance.GetMapLeaderboard();

        //Allocate the array
        highscoreItems = new SetLeaderboardItemInfo[currentMapLeaderboard.leaderboard.Count];
        for(int i = 0; i < highscoreItems.Length; i++){
            //Instantiate an leaderboard items inside the leaderboard
            GameObject leaderboardGameObject = Instantiate(highscoreItemPrefab, leaderboardLayout.transform);

            highscoreItems[i] = leaderboardGameObject.GetComponent<SetLeaderboardItemInfo>();

            highscoreItems[i].SetPositionText($"{i + 1}.");
            
        }

        UpdateLeaderboard();
    }
}
