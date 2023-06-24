using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public int curHealth;


    [Header("Player Maximum Stats")]
    public int maxHealth;

    [Header("Components")]
    public TopDownCarController carController;


    // Start is called before the first frame update
    void OnEnable() {
        carController = GetComponent<TopDownCarController>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
