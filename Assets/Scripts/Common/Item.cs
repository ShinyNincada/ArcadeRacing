using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public abstract class Item : MonoBehaviour{
    //Common properties
    public int itemId;
    public string itemName;

    public abstract void  Use(TopDownCarEffectHandler effectHandler);

    public abstract void Collect(TopDownCarEffectHandler effectHandler);

    private void OnTriggerEnter2D(Collider2D col) {
         if(col.transform.parent.CompareTag("Player") || col.transform.parent.CompareTag("AI")){
            Collect(col.transform.parent.GetComponent<TopDownCarEffectHandler>());
            LeanPool.Despawn(this);
        }
    }
}
