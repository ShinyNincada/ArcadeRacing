using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    Animator _anim;
    float timer = 0.4f;
    // Start is called before the first frame update
    void OnEnable() {
        _anim = GetComponent<Animator>();
    }

    private void Update() {
        timer -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.CompareTag("Player")){
            if(timer < 0){
                _anim.Play("TireTrigger");
                Rigidbody2D rb2D = col.gameObject.GetComponent<Rigidbody2D>();
                rb2D.AddForce(rb2D.velocity * -7, ForceMode2D.Impulse);
                timer = 0.4f;
            }
        }
    }
}
