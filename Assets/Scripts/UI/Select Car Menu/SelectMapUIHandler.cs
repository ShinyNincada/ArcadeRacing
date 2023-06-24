using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System.Threading.Tasks;
public class SelectMapUIHandler : MonoBehaviour
{
    [Header("Map Prefab")]
    public GameObject mapPrefab;

    [Header("Spawn On")]
    public Transform spawnOnTransform;

    bool isChanging = false;

    MapData[] mapDatas;
    int selectedMapIndex = 0;

    //Other component
    MapUIHandler mapUIHandler;
    LockSelected mapLocker;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        //Load cardata
        mapDatas = Resources.LoadAll<MapData>("MapData/");
        mapLocker = GetComponent<LockSelected>();
        SpawnMap(true);
    }

    void Update()
    {   
        if(Input.GetKey(KeyCode.LeftArrow)){
            OnPreviousMap();
        }
        else if(Input.GetKey(KeyCode.RightArrow)){
            OnNextMap();
        }
        else if(Input.GetKeyDown(KeyCode.Space)){
            OnSelectMap();
        }
    }

    async void SpawnMap(bool isMapAppearFromRightSide){
        isChanging = true;

        //Nếu đã tồn tại mapUIHandler => di chuyển ra phía ngược lại với đầu vào
        if(mapUIHandler != null){
            mapUIHandler.StartMapExitAnimation(!isMapAppearFromRightSide);
        }

        GameObject map = LeanPool.Spawn(mapPrefab, spawnOnTransform);

        mapUIHandler = map.GetComponent<MapUIHandler>();
        mapUIHandler.StartMapEntranceAnimation(isMapAppearFromRightSide);
        mapUIHandler.SetupMap(mapDatas[selectedMapIndex]);
        string key = $"MapUnlocked_{mapDatas[selectedMapIndex].MapUniqueID}";
        bool isUnlocked = ES3.KeyExists(key)? ES3.Load<bool>(key) : false;
        mapLocker.SetupValue(isUnlocked, mapDatas[selectedMapIndex].lockValue, key);
        await Task.Delay(500);

        isChanging = false;
    }

    public void OnPreviousMap(){

        if(isChanging) 
            return;

        selectedMapIndex--;

        if(selectedMapIndex < 0)
            selectedMapIndex = mapDatas.Length - 1;
        SpawnMap(true);
    }

    public void OnNextMap(){
         
        if(isChanging) 
            return;
            
        selectedMapIndex++;

        if(selectedMapIndex > mapDatas.Length-1)
            selectedMapIndex = 0;
        SpawnMap(false);
    }

    public void OnSelectMap(){
        GameManager.instance.ClearDriverList();

        GameManager.instance.SetSelectedMap(mapDatas[selectedMapIndex]);
    }
}
