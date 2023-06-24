using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Car Data", menuName = "Car Data", order = 51)]
public class CarData : ScriptableObject
{
    [SerializeField]
    private int carUniqueID = 0;

    [SerializeField]
    private string carName;
    
    [SerializeField]
    private Sprite carUISprite;

    [SerializeField]
    private GameObject carPrefab;

    public float CarSpeed;
    public float CarAcceleration;
    public float Drift;
    public int CarUniqueID{
        get { return carUniqueID; }
    }

    public Sprite CarUISprite{
        get { return carUISprite; }
    }

    public GameObject CarPrefab{
        get { return carPrefab; }
    }
    public string CarName{
        get { return carName; }
    }
}
