using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostCarPlayBack : MonoBehaviour
{
    //local variables
    GhostCarData ghostCarData = new GhostCarData();
    List<GhostCarReplay> GhostCarReplayList = new List<GhostCarReplay>();

    //Playback information
    float lastStoredTime = 0.1f;
    Vector2 lastStoredPosition = Vector2.zero;

    float lastStoredRotation = 0;
    Vector3 lastStoredLocalScale = Vector3.zero;
    //Playback index
    int currentPlaybackIndex = 0;
    
    //Duration of the data frame
    float duration = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //We can only playback if there is some data
        if(currentPlaybackIndex == 0)
            return;
        
        if(Time.timeSinceLevelLoad >= GhostCarReplayList[currentPlaybackIndex].timeSinceLevelLoaded){

            lastStoredTime = GhostCarReplayList[currentPlaybackIndex].timeSinceLevelLoaded;
            lastStoredPosition = GhostCarReplayList[currentPlaybackIndex].position;
            lastStoredRotation = GhostCarReplayList[currentPlaybackIndex].rotationZ;
            lastStoredLocalScale = GhostCarReplayList[currentPlaybackIndex].localScale;
            
            //Step to the next item
            if(currentPlaybackIndex < GhostCarReplayList.Count - 1){
                currentPlaybackIndex++;
            }

            duration = GhostCarReplayList[currentPlaybackIndex].timeSinceLevelLoaded - lastStoredTime;
        }

        //Calculated how much of data frame that we have completed
        float timePassed = Time.timeSinceLevelLoad - lastStoredTime;
        float lerpPercentage = timePassed / duration;

        //Lerp everything
        transform.position  = Vector2.Lerp(lastStoredPosition, GhostCarReplayList[currentPlaybackIndex].position, lerpPercentage);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, lastStoredRotation), Quaternion.Euler(0, 0, GhostCarReplayList[currentPlaybackIndex].rotationZ), lerpPercentage);
        transform.localScale = Vector3.Lerp(lastStoredLocalScale, GhostCarReplayList[currentPlaybackIndex].localScale, lerpPercentage);

    }

    public void LoadData(int playerNumber){
        if(!PlayerPrefs.HasKey($"{SceneManager.GetActiveScene().name}_{playerNumber}_ghost")){
            Destroy(gameObject);
        }
        else{
            string jsonEncodeData = PlayerPrefs.GetString($"{SceneManager.GetActiveScene().name}_{playerNumber}_ghost");

            ghostCarData = JsonUtility.FromJson<GhostCarData>(jsonEncodeData);
            GhostCarReplayList = ghostCarData.GetDataList();
        }
    }
}
