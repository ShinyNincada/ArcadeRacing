using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class LapCounterUIHandler : MonoBehaviour
{
    TMP_Text LapText;
    float lastTimeCompleted = 0f;
    public List<int> LapTimes = new List<int>();
    private void Awake() {
        LapText = GetComponent<TMP_Text>();
    }

    public void SetLapText(string text){
        Sequence sequence = DOTween.Sequence();
        sequence.Append(LapText.transform.DOScale(3, 0.5f))
            .Append(LapText.transform.DOScale(1, 1f)).SetEase(Ease.OutBounce);
        LapText.text = text;
        LapTimes.Add(Mathf.FloorToInt(Time.time - lastTimeCompleted));
        lastTimeCompleted = Time.time;
    }
}
