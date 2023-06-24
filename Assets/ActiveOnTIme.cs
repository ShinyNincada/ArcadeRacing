using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnTIme : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.instance.gameMode != GameManager.MODE.TIME){
            this.gameObject.SetActive(false);
        }
        else{
            this.gameObject.SetActive(true);
        }
    }
}
