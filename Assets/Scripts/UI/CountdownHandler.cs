using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using DG.Tweening;

public class CountdownHandler : MonoBehaviour
{
    public TMP_Text countdownText;
    void Awake(){
        countdownText.text = "";
    }

    void Start(){
        Countdown();
    }

    async void Countdown(){
        if (Time.timeScale == 0)
        {
            return; // Exit early if time scale is 0
        }
        countdownText.text = "Ready!!";
        await Task.Delay(500);
        GameManager.instance.UpdateGameState(GameManager.GameState.COUNTDOWN);
        int counter = 3;
        while(true){
            
            if(counter != 0){
                countdownText.text = counter.ToString();
            }
            else{
                countdownText.text = "GO!";
                GameManager.instance.onRaceStart();
                break;
            }

            Sequence tweening = DOTween.Sequence();
            tweening.Append(countdownText.transform.DOScale(3, 0.6f))
                    .Append(countdownText.DOFade(0, 0.3f));

            counter--;
            await Task.Delay(1000);
            countdownText.transform.localScale = Vector3.one;
            countdownText.color = Color.black;
        }

        await Task.Delay(300);
        gameObject.SetActive(false);
    }

   
}
