using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starup
{
    [RuntimeInitializeOnLoadMethod]
    public static void InstantiatePrefabs(){
        Debug.Log("--- Spawning Objects ---");
        
        GameObject[] prefabs = Resources.LoadAll<GameObject>("SpawnOnLoad/");

        foreach(GameObject prefab in prefabs){
            Debug.Log($"Creating {prefab.name}");

            GameObject.Instantiate(prefab);
        }
        Debug.Log("--- Spawning Objects DONE---");
    }
}
