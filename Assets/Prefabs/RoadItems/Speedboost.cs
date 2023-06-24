using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedboost : MonoBehaviour
{
    [Header("Boost Stats")]
    [SerializeField] float boostPower = 5f;
    [SerializeField] float boostDuration = 1.5f;
    private void OnTriggerEnter2D(Collider2D col) {
        if(col.transform.parent.tag == "Player" || col.transform.parent.tag == "AI"){
            //Take the car controller
            TopDownCarEffectHandler effectHandler = col.GetComponentInParent<TopDownCarEffectHandler>();
            // effectHandler.OilSlickEffect(boostPower, boostDuration);
            Destroy(this.gameObject);
        }
    }
}
