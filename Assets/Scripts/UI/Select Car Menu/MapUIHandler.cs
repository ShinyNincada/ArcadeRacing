using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Lean.Pool;
public class MapUIHandler : MonoBehaviour
{
    [Header("Map Image")]
    public Image mapImage;
    [Header("Map Name")]
    public TMP_Text mapText;
    Animator _ani = null;
    
    private void OnEnable() {
        _ani = GetComponent<Animator>();
    }

    public void StartMapEntranceAnimation(bool isCarAppearFromRightSide){
        if(isCarAppearFromRightSide){
            _ani.Play("Map UI Appear From Right");
        }
        else{
            _ani.Play("Map UI Appear From Left");
        }
    }

    public void StartMapExitAnimation(bool isExitToTheRight){
        if(isExitToTheRight){
            _ani.Play("Map UI Disappear To Right");
        }
        else{
            _ani.Play("Map UI Disappear To Left");
        }
    }

    public void SetupMap(MapData mapData){
        mapImage.sprite = mapData.MapUISprite;
        mapText.text = mapData.MapName;
    }

    //Events 
    public void OnMapExitAnimationCompleted(){
        LeanPool.Despawn(gameObject);
    }
}
