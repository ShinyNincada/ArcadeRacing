using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GhostCarRecorder : MonoBehaviour
{
    public Transform carSpriteObject;
    public GameObject ghostCarPref;

    //Local variables
    GhostCarData ghostCarData = new GhostCarData();

    bool isRecording = false;

    //Other Components
    Rigidbody2D carRb2d;
    CarInputHandler carInput;

    private void Awake() {
        carRb2d = GetComponent<Rigidbody2D>();
        carInput = GetComponent<CarInputHandler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //Create a ghost car
        GameObject ghostCar = Instantiate(ghostCarPref);
        ghostCar.GetComponent<GhostCarPlayBack>().LoadData(carInput.playerNumber);
        StartCoroutine(RecordCarPositionCO());   
        StartCoroutine(SaveCarPosition());   
    }

    IEnumerator RecordCarPositionCO(){

        while(isRecording){
            if(carSpriteObject != null){
                ghostCarData.AddDataItem(new GhostCarReplay(carRb2d.position, carRb2d.rotation, carSpriteObject.localScale, Time.timeSinceLevelLoad));
            }
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator SaveCarPosition(){
        yield return new WaitForSeconds(5f);

        SaveData();
    }
    void SaveData(){
        string jsonEncodeData = JsonUtility.ToJson(ghostCarData);
        print($"Saved ghost data to json files {jsonEncodeData}");

        if(carInput != null){
            PlayerPrefs.SetString($"{SceneManager.GetActiveScene().name}_{carInput.playerNumber}_ghost", jsonEncodeData);
            PlayerPrefs.Save();
        }

        isRecording = false;    
    }
}
