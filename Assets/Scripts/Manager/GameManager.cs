using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private GameState _state;

    //Gold
    public int gold;
    public string playerName = "P12212";
    //Time
    float raceStartedTime = 0;
    float raceCompletedTime = 0;

    //Driver info
    List<DriverInfo> driverInfoList = new List<DriverInfo>();

    //Map info
    MapData selectedMap;
    public MODE gameMode;
    
    //Other component
    public GoldManager _goldManager;
    public event Action<GameManager> OnGameStateChanged;
    private void Awake() {
        if(instance == null){
            instance = this;
        }
        else if(instance != null){
            Destroy(gameObject);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        _goldManager = GameObject.FindObjectOfType<GoldManager>();
       
        DontDestroyOnLoad(this.gameObject);

        //Supply dummy driver information for testing purpose
        driverInfoList.Add(new DriverInfo(1, playerName, 1, false));
    }

    void Update(){
        if(_goldManager == null){
            _goldManager = FindObjectOfType<GoldManager>();
        }
    }

    void Start(){
        gold = SaveManager.instance.gold;
    }
    
    public enum GameState{
        SELECTCAR,
        SELECTMAP,
        SETTINGS,
        COUNTDOWN,
        PLAYING,
        PAUSE,
        GAMEOVER,
        QUIT
    }

      public enum MODE {
        TIME,
        RACE,
        SURVIVOR
    }

    public void UpdateGameState(GameState newState){
        _state = newState;

        switch(_state){
            
            case GameState.COUNTDOWN:
                break;
            case GameState.PLAYING:
                Time.timeScale = 1;
                break;
            case GameState.GAMEOVER:
                break;
            case GameState.PAUSE:
                Time.timeScale = 0;
                break;
            case GameState.SELECTCAR:
                break;
            case GameState.SELECTMAP:
                break;
            case GameState.SETTINGS:
                break;
            case GameState.QUIT:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        //Invoke game state change event
        OnGameStateChanged?.Invoke(this);

    }

    public void PlayAgain(){
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextScene(string newScene){
        //Reset the time scale each time start a scene;
        Time.timeScale = 1;
        SceneManager.LoadScene(newScene);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        LevelStart();
    }

    public void LevelStart(){
        UpdateGameState(GameState.COUNTDOWN);
    }

    public void onRaceStart(){
        Debug.Log("On Race Start");

        raceStartedTime = Time.time;
        UpdateGameState(GameState.PLAYING);
    }

    public void OnRaceCompleted(){
        Debug.Log("On Game Completed");

        raceCompletedTime = Time.time;

        UpdateGameState(GameState.GAMEOVER);

        SaveGold();
    }

    public GameState GetGameState(){
        return _state;
    }

    public float getRaceTime(){

        if(_state == GameState.COUNTDOWN){
            return 0;
        }
        else if(_state == GameState.GAMEOVER){
            return raceCompletedTime - raceStartedTime;
        }
        else return Time.time - raceStartedTime;
    }

    public List<DriverInfo> GetDriverList(){
        return driverInfoList;
    }

    public void ClearDriverList(){
        driverInfoList.Clear();
    }
    
    //Driver information handling
    public void AddDriver(int playerNumber, string name, int carUniqueId, bool isAI){
        driverInfoList.Add(new DriverInfo(playerNumber, name, carUniqueId, isAI));
    }

    DriverInfo FindDriverInfo(int playerNumber){
        foreach(DriverInfo info in driverInfoList){
            if(playerNumber == info.PlayerNumber){
                return info;
            }
        }

        Debug.LogError($"Couldn't find driver info with playernumber = {playerNumber}");
        return null;
    }

    public void SetDriverLastRacePosition(int playerNumber, int position){
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        driverInfo.lastRacePosition = position;   
    }

    public void AddPointsToChampion(int playerNumber, int point){
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        driverInfo.championPoints += point;
    }

    public void SetNewMapLeaderboard(string name, float time){
        int id = selectedMap.MapUniqueID;
        SaveManager.instance.SaveLeaderboard(id, name, time);
    }

    public SaveManager.MapLeaderboard GetMapLeaderboard(){
  
        SaveManager.MapLeaderboard currentLeaderboard = SaveManager.instance.LoadLeaderboard(selectedMap.MapUniqueID);
        return currentLeaderboard;
    }
    public void SetSelectedMap(MapData map){
        selectedMap = map;
    }

    public String GetSelectedMap(){
        return selectedMap.MapScene;
    }

    public void SetGameMode(MODE newMode){
        gameMode = newMode;
        switch(newMode){
            case MODE.TIME:
                ES3.Save<int>("Mode", 1);
                break;
            case MODE.RACE:
                ES3.Save<int>("Mode", 2);
                break;
            case MODE.SURVIVOR:
                ES3.Save<int>("Mode", 3);
                break;
            default:
                ES3.Save<int>("Mode", 1);
                break;
        }
        
    }

    public void TakeGold(Transform pos, int value){
        _goldManager.GoldCollected(pos, value);
        gold = _goldManager.Golds;

    }

    public void SpendGold(int value){
        _goldManager.SpendGold(value);
        SaveGold();
    }

    public void SaveGold(){
        gold = _goldManager.Golds;
        SaveManager.instance.gold = gold;
        SaveManager.instance.Save();
    }
}

