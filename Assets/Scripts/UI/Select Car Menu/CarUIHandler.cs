using UnityEngine;
using Lean.Pool;
using UnityEngine.UI;
using TMPro;
public class CarUIHandler : MonoBehaviour
{   
    [Header("Car Image")]
    public Image carImage;
    [Header("Car Name")]
    public TMP_Text carText;
    Animator _ani = null;

    private void OnEnable() {
        _ani = GetComponent<Animator>();
    }

    public void StartCarEntranceAnimation(bool isCarAppearFromRightSide){
        if(isCarAppearFromRightSide){
            _ani.Play("Car UI Appear From Right");
        }
        else{
            _ani.Play("Car UI Appear From Left");
        }
    }

    public void StartCarExitAnimation(bool isExitToTheRight){
        if(isExitToTheRight){
            _ani.Play("Car UI Disappear To Right");
        }
        else{
            _ani.Play("Car UI Disappear To Left");
        }
    }

    public void SetupCar(CarData carData){
        carImage.sprite = carData.CarUISprite;
        carText.text = carData.CarName;
    }

    //Events 
    public void OnCarExitAnimationCompleted(){
        LeanPool.Despawn(gameObject);
    }
}
