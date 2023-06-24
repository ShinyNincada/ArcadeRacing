using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeHeightObject : MonoBehaviour
{
    [Header("Partial Transform")]
    public Transform objTransform;
    public Transform bodyTransform;
    public Transform shadowTransform;

    public float gravity = -10;
    public Vector2 groundVelocity;
    public float verticalVelocity;
    public bool isGrounded;
  
  
    public void Initialize(Vector2 groundVel, float verticalVelocity){

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }


    void UpdatePosition(){

    }
}
