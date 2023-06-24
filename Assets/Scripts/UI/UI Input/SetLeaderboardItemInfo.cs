using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SetLeaderboardItemInfo : MonoBehaviour
{
    public TMP_Text positionText;
    public TMP_Text nameText;
    public TMP_Text timeText;

    public void SetPositionText(string newPosition){
        positionText.text = newPosition;
    }

    public void SetNameText(string newName){
        nameText.text = newName;
    }

    public void SetTimeText(float time){
        timeText.text = time.ToString();
    }
}
