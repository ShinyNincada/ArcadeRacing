using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System.Threading.Tasks;
using UnityEngine.UI;
using DG.Tweening;

public class SelectCarUIHandler : MonoBehaviour
{
    [Header("Car Prefab")]
    public GameObject carPrefab;

    [Header("Spawn On")]
    public Transform spawnOnTransform;

    [Header("Sliders")]
    public Slider speedSlider;
    public Slider accelSlider;
    public Slider driftSlider;

    bool isChanging = false;

    CarData[] carDatas;
    int selectedCarIndex = 0;

    //Other components
    CarUIHandler carUIHandler = null;
    

    // Start is called before the first frame update
    void OnEnable()
    {
        //Load cardata
        carDatas = Resources.LoadAll<CarData>("CarData/");
        SpawnCar(true);
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetKey(KeyCode.LeftArrow)){
            OnPreviousCar();
        }
        else if(Input.GetKey(KeyCode.RightArrow)){
            OnNextCar();
        }
        else if(Input.GetKeyDown(KeyCode.Space)){
            OnSelectCar();
        }
    }

    async void SpawnCar(bool isCarAppearFromRightSide){
        isChanging = true;

        //Nếu đã tồn tại carUIHandler => di chuyển ra phía ngược lại với đầu vào
        if(carUIHandler != null){
            carUIHandler.StartCarExitAnimation(!isCarAppearFromRightSide);
        }

        GameObject car = LeanPool.Spawn(carPrefab, spawnOnTransform);

        carUIHandler = car.GetComponent<CarUIHandler>();
        carUIHandler.StartCarEntranceAnimation(isCarAppearFromRightSide);
        carUIHandler.SetupCar(carDatas[selectedCarIndex]);
        UpdateCarStats();
        await Task.Delay(500);

        isChanging = false;
    }

    public void OnPreviousCar(){

        if(isChanging) 
            return;

        selectedCarIndex--;

        if(selectedCarIndex < 0)
            selectedCarIndex = carDatas.Length - 1;
        SpawnCar(true);
    }

    public void OnNextCar(){
         
        if(isChanging) 
            return;
            
        selectedCarIndex++;

        if(selectedCarIndex > carDatas.Length-1)
            selectedCarIndex = 0;
        SpawnCar(false);
    }

    public void OnSelectCar(){
        GameManager.instance.ClearDriverList();

        GameManager.instance.AddDriver(1, GameManager.instance.playerName, carDatas[selectedCarIndex].CarUniqueID, false);
        SaveManager.instance.carSelectedId = selectedCarIndex;
        //Create a new list of cars
        List<CarData> uniqueCars = new List<CarData>(carDatas);

        uniqueCars.Remove(carDatas[selectedCarIndex]);

        string[] names = {"BOT1", "BOT2", "StillBot", "OnlyBot", "Minh mo", "Adamant"};
        List<string> uniqueNames = new List<string>(names);

        for(int i = 2; i < 3; i++){
            string driverName = uniqueNames[Random.Range(0, uniqueNames.Count)];
            uniqueNames.Remove(driverName);

            CarData carData = uniqueCars[Random.Range(0, uniqueCars.Count)];
            
            GameManager.instance.AddDriver(i, driverName, carData.CarUniqueID, true);
        }

        GameManager.instance.NextScene(GameManager.instance.GetSelectedMap());
    }

    public void UpdateCarStats(){
        speedSlider.DOValue(carDatas[selectedCarIndex].CarSpeed, 1f);
        accelSlider.DOValue(carDatas[selectedCarIndex].CarAcceleration, 1f);
        driftSlider.DOValue(carDatas[selectedCarIndex].Drift, 1f);
    }
}
