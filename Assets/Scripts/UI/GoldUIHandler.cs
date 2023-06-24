using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GoldUIHandler : MonoBehaviour
{   
    public TMP_Text goldText;

    int _c;
    
    //Update gias
    public int Golds{
        get {return _c; }
        set{
            _c = value;
            goldText.text = Golds.ToString();
        }
    }
    void Start(){
         goldText.text = SaveManager.instance.gold.ToString();
    }

    public void AddGold(int value){
        Golds += value;
    }

}
