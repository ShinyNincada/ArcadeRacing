using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class SnowBullet : MonoBehaviour
{
    public float  timerCD = 10f;
    public GameObject snowEffect;
    public Rigidbody2D rb;
    Animator _ani;

    private void OnEnable() {
        _ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ColDisableFor05();
        
    }
    private void Update() {
        if(timerCD > 0){
            timerCD -= Time.deltaTime;
        }    
        else{
            _ani.SetTrigger("Trigger");
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Player") || col.CompareTag("AI")){
            var particles = LeanPool.Spawn(snowEffect, transform.position, transform.rotation);
            LeanPool.Despawn(particles, 1f);
            TopDownCarEffectHandler ef = col.transform.parent.GetComponent<TopDownCarEffectHandler>();
            ef.StunEffect(2f);
        }
    }

    void DespawnThis(){
        LeanPool.Despawn(gameObject, 0.3f);
    }

    public async void ColDisableFor05(){
        GetComponent<Collider2D>().enabled = false;
        await System.Threading.Tasks.Task.Delay(500);
        GetComponent<Collider2D>().enabled = true;

    }
}
