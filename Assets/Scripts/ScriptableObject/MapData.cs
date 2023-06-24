using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Data", menuName = "Map Data", order = 52)]
public class MapData : ScriptableObject
{
  

    [SerializeField]
    private int mapUniqueID = 0;

    [SerializeField]
    private string mapName;
    
    [SerializeField]
    private Sprite mapUISprite;

    [SerializeField]
    private string mapScene;
    
    [SerializeField]
    private GameManager.MODE mapMode;

    public int lockValue;
    public int MapUniqueID{
        get { return mapUniqueID; }
    }

    public Sprite MapUISprite{
        get { return mapUISprite; }
    }

    public GameManager.MODE MapPrefab{
        get { return mapMode; }
    }
    public string MapName{
        get { return mapName; }
    }
    public string MapScene{
        get { return mapScene; }
    }
}
