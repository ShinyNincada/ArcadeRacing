using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
public class Bullet : MonoBehaviour
{
    Animator _anim;
    Rigidbody2D _rb2d;
    public float timer = 10f;
    public float speed = 5f;
    public GameObject target = null;

    // Start is called before the first frame update
    void OnEnable()
    {
        _anim = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();
        timer = 10f;
    }

    private void Update() {
        if(target != null){
            FollowUp(target);
        }
        if(timer < 0){
            Trigger();
        }
        timer -= Time.deltaTime;
    }
    void Trigger(){
        _anim.Play("Missile_Destroy");
    }
    public void DeSpawnThis(){
        LeanPool.Despawn(this.gameObject, 0.2f);
    }

    public void FollowUp(GameObject target){
         // Calculate the direction towards the enemy
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // Apply force to the projectile to make it move towards the enemy
            _rb2d.velocity = direction * speed;

            // Rotate the projectile to face the direction of movement
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bullet")){
            Trigger();
            GameManager.instance.OnRaceCompleted();
        }
    }
}
