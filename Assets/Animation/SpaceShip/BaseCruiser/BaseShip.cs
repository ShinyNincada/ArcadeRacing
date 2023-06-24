using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using DG.Tweening;
public class BaseShip : MonoBehaviour
{
    public int Health = 3;
    public bool isShield = false;
    public GameObject projectile = null;
    public List<Transform> shootPoint;
    public GameObject target;
   
    // Components
    Animator _anim;
    
    void OnEnable()
    {
        Health = 3;
        _anim = GetComponent<Animator>();
       
    }

    void Start(){
         target = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void LaunchProjectiles(){
        foreach(Transform point in shootPoint){
            GameObject bulletGO = LeanPool.Spawn(projectile, point.position, point.rotation);
            bulletGO.GetComponent<Rigidbody2D>().AddForce(point.up * 200, ForceMode2D.Force);
            bulletGO.GetComponent<Bullet>().target = target;
            
        }
    }



    private void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Bullet")){
            if(!isShield){
                Health -= 1;
                if(Health < 0){
                    _anim.SetTrigger("Destroyed");
                }
                else{
                    _anim.SetTrigger("TakeDamage");
                }
            }
        }
    }

    public void DeSpawnThis(){
        LeanPool.Despawn(gameObject, 0.2f);
    }
}
