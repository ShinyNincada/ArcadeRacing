using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class GameOverUIHandler : MonoBehaviour
{
    //Other component
    Canvas canvas;
    public Transform Leaderboard;
    

    private void Start() {
        canvas = GetComponent<Canvas>();

        canvas.enabled = false;

        GameManager.instance.OnGameStateChanged += OnGameStateChanged;
    }
  
    public void OnPlayAgain(){
        GameManager.instance.PlayAgain();
    }

    public void OnBackToMenu(){
        GameManager.instance.NextScene("GameStart");
    }

    void OnGameStateChanged(GameManager gameManager){
        if(GameManager.instance.GetGameState() == GameManager.GameState.GAMEOVER){
            ShowGameOverMenu();
        }
    }

    async void ShowGameOverMenu(){

        await Task.Delay(1000);

        canvas.enabled = true;
        Sequence sequence = DOTween.Sequence(); 
        
        sequence.Append(canvas.transform.DOScale(3, 0.5f))
            .Append(canvas.transform.DOScale(1, 1f)).SetEase(Ease.OutBounce);

        await Task.Delay(1000);

        for(int i = 0; i < Leaderboard.transform.childCount; i++){
            Transform leaderboardItem =  Leaderboard.transform.GetChild(i).transform;
            leaderboardItem.localScale = Vector3.zero;

            sequence.Append(leaderboardItem.DOScale(Vector3.one, 0.5f).SetDelay(i * 0.1f).SetEase(Ease.OutBack));
        }
        
    }
            

    
    //unhook event when destroy the menu 
    private void OnDestroy() {
        GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
    }

    
}
