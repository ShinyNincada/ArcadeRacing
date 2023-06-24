using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GoldCoin : Item
{
    public int value = 1;
    public GoldCoin(){
        
    }
    
    public override void Use(TopDownCarEffectHandler effectHandler){
        GameManager.instance.TakeGold(effectHandler.transform, value);
    }

    public override void Collect(TopDownCarEffectHandler effectHandler){
        if(effectHandler.gameObject.tag == "Player"){
            Use(effectHandler);
        }
    }
}
