using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarUIInputHandler : MonoBehaviour
{
    CarInputHandler playerCarInput;
    Vector2 inputVector = Vector2.zero;

    public Canvas settingCanvas;
    private void Awake() {

        //Chon ra xe dau tien su dung UIinput
        if(FindObjectsOfType<CarInputHandler>() != null)
            playerCarInput = FindObjectsOfType<CarInputHandler>().First(s => s.isUIInput);
    }
    
    public void OnAcceleratePress(){
        inputVector.y = 1.0f;
        playerCarInput.SetInput(inputVector);
    }
 
    public void OnAccelerateRelease(){
        inputVector.y = 0f;
        playerCarInput.SetInput(inputVector);

    }

    public void OnBrakePress(){
        inputVector.y = -1.0f;
        playerCarInput.SetInput(inputVector);

    }

    public void OnBrakeRelease(){
        inputVector.y = 0f;
        playerCarInput.SetInput(inputVector);

    }

    public void OnSteerLeftPress(){
        inputVector.x = -1.0f;
        playerCarInput.SetInput(inputVector);

    }
    public void OnSteerLeftRelease(){
        inputVector.x = 0f;
        playerCarInput.SetInput(inputVector);

    }
    public void OnSteerRightPress(){
        inputVector.x = 1.0f;
        playerCarInput.SetInput(inputVector);

    }
    public void OnSteerRightRelease(){
        inputVector.x = 0f;
        playerCarInput.SetInput(inputVector);

    }

    public void Pause(){
        if(GameManager.instance.GetGameState() != GameManager.GameState.PAUSE){
            GameManager.instance.UpdateGameState(GameManager.GameState.PAUSE);
            settingCanvas.enabled = true;
        }
        else{
            GameManager.instance.UpdateGameState(GameManager.GameState.PLAYING);
            settingCanvas.enabled = false;
        }
    }
}
