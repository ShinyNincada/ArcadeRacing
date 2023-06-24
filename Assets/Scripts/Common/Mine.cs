using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Item
{
    public GameObject minePrefab;

    public override void Use(TopDownCarEffectHandler effectHandler){
        
    }

    public override void Collect(TopDownCarEffectHandler effectHandler){
        effectHandler.AddItem(base.itemId);
        effectHandler.item = minePrefab;
    }
}
