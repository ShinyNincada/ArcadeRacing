using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFreeze : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float Power = 5f;
    [SerializeField] float Duration = 1.5f;
    private void OnTriggerEnter2D(Collider2D col) {
        if(col.transform.parent.tag == "Player" || col.transform.parent.tag == "AI"){
            //Take the car controller
            TopDownCarEffectHandler effectHandler = col.GetComponentInParent<TopDownCarEffectHandler>();
          
            Destroy(this.gameObject);
        }
    }
}
