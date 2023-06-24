using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class LockSelected : MonoBehaviour
{
    public Canvas lockedCanvas;
    public Image lockedImage;
    public TMP_Text valueText;
    public Image valueImg;
    public int lockedValue;
    public string currentKey;
    public void Unlock(){
        if(GameManager.instance.gold >= lockedValue){
            Unlocking();
        }
        else{
            LockingAnimation();
        }
    }

    void LockingAnimation(){
        ES3.Save<bool>(currentKey, false);
        Sequence wrongSequence = DOTween.Sequence();
        wrongSequence.Append(lockedImage.transform.DORotate(new Vector3(0, 0, 30), 0.3f))
        .Append(lockedImage.transform.DORotate(new Vector3(0, 0, -30), 0.3f))
        .SetLoops(3).SetEase(Ease.OutBack)
        .OnComplete(() => {lockedImage.transform.DORotate(Vector3.zero, 0.2f);});
    }

    void Unlocking(){
        GameManager.instance.SpendGold(lockedValue);
        ES3.Save<bool>(currentKey, true);
        lockedCanvas.enabled = false;
    }

    public void SetupValue(bool unlocked, int value, string key){
        currentKey = key;
        if(unlocked){
            lockedCanvas.enabled = false;
        }
        else{ 
            lockedCanvas.enabled = true;
            lockedValue = value;
            valueText.text = lockedValue.ToString();
            if(lockedValue > GameManager.instance.gold){
                valueText.color = Color.red;
                valueImg.color = Color.red;
            }
            else{
                valueText.color = Color.green;
                valueImg.color = Color.green;
            }
        }
        
    }
}
