using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLayerHandler : MonoBehaviour
{
    //Components
    List<SpriteRenderer> defaultSpriteRenders = new List<SpriteRenderer>();
    List<Collider2D> overpassCols = new List<Collider2D>();
    List<Collider2D> underpassCols = new List<Collider2D>();
    Collider2D carCollider;
    
    public SpriteRenderer carOutlineRender;
    //variables
    bool isDrivingOverpass = false;
    

    private void Awake()
    {
        foreach (SpriteRenderer renderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if(renderer.sortingLayerName == "Default"){
                defaultSpriteRenders.Add(renderer);
            }
        }

        foreach(GameObject col in GameObject.FindGameObjectsWithTag("OverpassCollider")){
            overpassCols.Add(col.GetComponent<Collider2D>());
        }
        
        foreach(GameObject col in GameObject.FindGameObjectsWithTag("UnderpassCollider")){
            underpassCols.Add(col.GetComponent<Collider2D>());
        }

        carCollider = GetComponentInChildren<Collider2D>();
    }

    private void Start() {
        UpdateLayer();
    }
   

   private void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("UnderpassTrigger")){
            isDrivingOverpass = false;
            UpdateLayer();
        }
    
        else if (col.CompareTag("OverpassTrigger")){
            isDrivingOverpass = true;
            UpdateLayer();
        }
   }

   void UpdateLayer(){
        if(isDrivingOverpass){
            SetSortingLayer("RaceTrackOverpass");

            carOutlineRender.enabled = false;
        }
        else{
            SetSortingLayer("Default");
            
            carOutlineRender.enabled = true;
        }

        SetCollisionWithOverPass();
   }

   void SetSortingLayer(string layerName){
    foreach(SpriteRenderer renderer in defaultSpriteRenders){
        renderer.sortingLayerName = layerName;
    }
   }

   void SetCollisionWithOverPass(){
        
        foreach(Collider2D col in overpassCols){
            Physics2D.IgnoreCollision(carCollider, col, !isDrivingOverpass);
        }
        
        foreach(Collider2D col in underpassCols){
            if(isDrivingOverpass){
                Physics2D.IgnoreCollision(carCollider, col, true);
            }
            else{
                Physics2D.IgnoreCollision(carCollider, col, false);
            }
        }
   }

    public bool IsDrivingOverpass()
    {
        return isDrivingOverpass;
    }

    
}
