using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : Item
{
    public GameObject oilPrefab;
    public override void Use(TopDownCarEffectHandler effectHandler){
  
    }

    public override void Collect(TopDownCarEffectHandler effectHandler){
        effectHandler.AddItem(base.itemId);
        effectHandler.item = oilPrefab;
    }

    
}
