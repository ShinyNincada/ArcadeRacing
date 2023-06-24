using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;
using Lean.Pool;
public class SpawnManager : MonoBehaviour
{
    List<DriverInfo> driverInfoList;
    public List<GameObject> itemPrefabList;
    public List<Transform> spawnPositionList;
    
    int numberOfCarSpawned = 0;
    public bool itemSpawner = false;
    // Start is called before the first frame update
    void Start()
    {   
        if(GameManager.instance.gameMode == GameManager.MODE.RACE){
            SpawnOnRace();
           
            
        }

        else if(GameManager.instance.gameMode == GameManager.MODE.TIME){
            SpawnOnTime();
            if (itemPrefabList.Count > 2)
            {
                itemPrefabList.RemoveRange(2, itemPrefabList.Count - 2);
            }
        }

        List<GameObject> findSpawnPos = GameObject.FindGameObjectsWithTag("ItemSpawner").ToList();
        if(findSpawnPos.Count > 0){
            foreach(var pos in findSpawnPos){
            spawnPositionList.Add(pos.transform);
            }
        }

        if(itemSpawner){
            InvokeRepeating("ItemSpawning", 5f, 10f);
        }
        Debug.Log(driverInfoList.Count);
    }

    public int GetNumberOfCarSpawned(){
        return numberOfCarSpawned;
    }

    void SpawnOnRace(){
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //Ensure that spawnPoint we Find same order as we place
        spawnPoints = spawnPoints.ToList().OrderBy(s => s.name).ToArray();

        //Load the car data
        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");
       
       //Driver info
        driverInfoList = new List<DriverInfo>(GameManager.instance.GetDriverList());
        driverInfoList = driverInfoList.OrderBy(s => s.lastRacePosition).ToList();
       
        for(int i = 0; i < spawnPoints.Length; i++){
            Transform spawnPoint = spawnPoints[i].transform;

            if(driverInfoList.Count == 0){
                return;
            }

            DriverInfo driverInfo = driverInfoList[0];
            int selectedCarID = driverInfo.carUniqueId;

            //Find the player car prefabs
            foreach(CarData carData in carDatas){
                if(carData.CarUniqueID == selectedCarID){
                    
                    //Now spawn it on the spawn point
                    GameObject car = Instantiate(carData.CarPrefab, spawnPoint.position, spawnPoint.rotation);
                    car.name = driverInfo.name;
                    car.GetComponent<CarInputHandler>().playerNumber = driverInfo.PlayerNumber;
                    
                    //if player is AI
                    if(driverInfo.isAI){
                        car.GetComponent<CarInputHandler>().enabled = false;
                        car.GetComponent<CarAIHandler>().enabled = true;
                        car.GetComponent<AIDestinationSetter>().enabled = true;
                        car.GetComponent<AIPath>().enabled = true;
                        car.tag = "AI";
                        car.transform.GetChild(2).tag = "AI";
                    }
                    else{
                        car.GetComponent<CarInputHandler>().enabled = true;
                        car.GetComponent<CarAIHandler>().enabled = false;
                        car.GetComponent<AIDestinationSetter>().enabled = false;
                        car.GetComponent<AIPath>().enabled = false;
                        car.tag = "Player";
                        car.transform.GetChild(2).tag = "Player";

                    }

                    numberOfCarSpawned++;
                    break;
                }
            }

            //Remove the spawned car
            driverInfoList.Remove(driverInfo);
        }
    }

    void SpawnOnTime(){
          GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //Ensure that spawnPoint we Find same order as we place
        spawnPoints = spawnPoints.ToList().OrderBy(s => s.name).ToArray();

        //Load the car data
        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");
       
       //Driver info
        driverInfoList = new List<DriverInfo>(GameManager.instance.GetDriverList());
        driverInfoList = driverInfoList.OrderBy(s => s.lastRacePosition).ToList();
       
        for(int i = 0; i < spawnPoints.Length; i++){
            Transform spawnPoint = spawnPoints[i].transform;

            if(driverInfoList.Count == 0){
                return;
            }

            DriverInfo driverInfo = driverInfoList[0];
            int selectedCarID = driverInfo.carUniqueId;

            //Find the player car prefabs
            foreach(CarData carData in carDatas){
                if(carData.CarUniqueID == selectedCarID){
                    
                    //Now spawn it on the spawn point
                    if(driverInfo.isAI)
                        break;
                    
                    GameObject car = Instantiate(carData.CarPrefab, spawnPoint.position, spawnPoint.rotation);
                    car.name = driverInfo.name;
                    car.GetComponent<CarInputHandler>().playerNumber = driverInfo.PlayerNumber;
                    
                    //if player is AI
                    if(driverInfo.isAI){
                        car.GetComponent<CarInputHandler>().enabled = false;
                        car.GetComponent<CarAIHandler>().enabled = true;
                        car.GetComponent<AIDestinationSetter>().enabled = true;
                        car.GetComponent<AIPath>().enabled = true;
                        car.tag = "AI";
                    }
                    else{
                        car.GetComponent<CarInputHandler>().enabled = true;
                        car.GetComponent<CarAIHandler>().enabled = false;
                        car.GetComponent<AIDestinationSetter>().enabled = false;
                        car.GetComponent<AIPath>().enabled = false;
                        car.tag = "Player";
                    }

                    numberOfCarSpawned++;
                    break;
                }
            }

            //Remove the spawned car
            driverInfoList.Remove(driverInfo);
        }
    }

    void ItemSpawning()
    {
        if (itemSpawner)
        {
            foreach (var pos in spawnPositionList)
            {
                // Check if there are any items already spawned at the position
                bool hasItem = CheckIfItemExistsAtPosition(pos);
                
                // Spawn item only if no item exists at the position
                if (!hasItem)
                {
                    SpawnItem(pos);
                }
            }
        }
    }

    bool CheckIfItemExistsAtPosition(Transform position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position.position);
        
        // Check if any of the colliders belong to an item object
        foreach (var collider in colliders)
        {
            if (collider.isTrigger && collider.CompareTag("Item"))
            {
                return true;
            }
        }
        
        return false;
    }

    void SpawnItem(Transform position)
    {
        // Choose a random item prefab from the list
        GameObject itemPrefab = itemPrefabList[Random.Range(0, itemPrefabList.Count)];

        // Instantiate the item prefab at the specified position
        GameObject newItem = LeanPool.Spawn(itemPrefab, position.position, Quaternion.identity);

        // Set the spawned item as a child of the spawner (optional)
        newItem.transform.parent = transform;
    }
}
