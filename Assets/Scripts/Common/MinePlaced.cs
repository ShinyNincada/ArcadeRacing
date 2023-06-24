using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
public class MinePlaced : MonoBehaviour
{
    public Animator _anim;
    public float timer;
    // Start is called before the first frame update
    void OnEnable() {
        _anim = GetComponent<Animator>();   
        timer = 10;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0){
            _anim.SetTrigger("Destroy");
        }
    }

    void DespawnThis(){
        LeanPool.Despawn(this, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.transform.parent.CompareTag("Player") || col.transform.parent.CompareTag("AI")){
             _anim.SetTrigger("Destroy");
            TopDownCarEffectHandler effectHandler = col.transform.GetComponentInParent<TopDownCarEffectHandler>();
            effectHandler.CarDestroyed();
        }
    }
}
