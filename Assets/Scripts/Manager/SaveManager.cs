using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    [Header("Map Unlocked")]
    public bool[] mapUnlocked = new bool[5] {true, false, false, false, false};

    [Header("Car Unlocked")]
    public bool[] carUnlocked = new bool[5] {true, true, false, false, false};

    [Header("Map leaderboard")]
    public List<MapLeaderboard> hello = new List<MapLeaderboard>();
    public int gold;
    public int carSelectedId;

    // Start is called before the first frame update
    void Awake() {
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(this);
        }

        LoadSettings();
        LoadMap();
        LoadGold();
    }
   
    public void Save(){
        ES3.Save<int>("gold", gold);
        ES3.Save<bool[]>("map", mapUnlocked);
    }

    public void LoadSettings(){

    }

    void LoadMap(){
        int modeSelected = ES3.Load<int>("Mode");
        switch(modeSelected){
            case 1:
                GameManager.instance.SetGameMode(GameManager.MODE.TIME);
                break;
            case 2:
                GameManager.instance.SetGameMode(GameManager.MODE.RACE);
                break;
            case 3:
                GameManager.instance.SetGameMode(GameManager.MODE.SURVIVOR);
                break;
            default:
                GameManager.instance.SetGameMode(GameManager.MODE.TIME);
                break;
        }
    }

    public void SaveLeaderboard(int mapId, string name, float finishedTime){
        LeaderboardItem newItem = new LeaderboardItem(name, finishedTime);        
        string key = "Leaderboard_" + mapId;
        MapLeaderboard currentLeaderboard;
        if(ES3.KeyExists(key)){
            currentLeaderboard = ES3.Load<MapLeaderboard>(key);
            currentLeaderboard.leaderboard.Add(newItem);
            
        }
        else{
            currentLeaderboard = new MapLeaderboard(mapId);
            currentLeaderboard.leaderboard.Add(newItem);
        }
       
      
        currentLeaderboard.leaderboard.Sort((a, b) => a.time.CompareTo(b.time));

        if(currentLeaderboard.leaderboard.Count > 10){
            currentLeaderboard.leaderboard = currentLeaderboard.leaderboard.GetRange(0, 10);
        }

        ES3.Save<MapLeaderboard>(key, currentLeaderboard);
    }

    public MapLeaderboard LoadLeaderboard(int id){
        string key = "Leaderboard_"+id;
        MapLeaderboard currentMapLeaderboard;
        if(ES3.KeyExists(key)){
            currentMapLeaderboard = ES3.Load<MapLeaderboard>(key);
        }
        else {
            currentMapLeaderboard = new MapLeaderboard(id);
            ES3.Save<MapLeaderboard>(key, currentMapLeaderboard);
        }
        return currentMapLeaderboard;
    }

    void LoadGold(){
        if(ES3.KeyExists("gold")){
            gold = ES3.Load<int>("gold");
        }
    }

    [System.Serializable]
    public class LeaderboardItem
    {
        public string name;
        public float time;

        public LeaderboardItem(){

        }
        public LeaderboardItem(string name, float time){
            this.name = name;
            this.time = time;
        }
    }

    [System.Serializable]
    public class MapLeaderboard
    {
        public int mapId;
        public List<LeaderboardItem> leaderboard;

        public MapLeaderboard(){

        }
        public MapLeaderboard(int id){
            mapId = id;
            leaderboard = new List<LeaderboardItem>();
        }
    }
}
