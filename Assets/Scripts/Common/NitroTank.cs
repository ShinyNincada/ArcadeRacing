using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NitroTank : Item
{
    public NitroTank(){
        
    }
    
    public override void Use(TopDownCarEffectHandler effectHandler){

    }

    public override void Collect(TopDownCarEffectHandler effectHandler){
        effectHandler.AddItem(base.itemId);
    }
}
